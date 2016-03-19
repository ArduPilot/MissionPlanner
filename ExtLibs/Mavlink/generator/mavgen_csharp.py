#!/usr/bin/env python
'''
parse a MAVLink protocol XML file and generate a C# implementation

Copyright Michael Oborne 2011
Released under GNU GPL version 3 or later
'''

import sys, textwrap, os, time, re
import mavparse, mavtemplate

t = mavtemplate.MAVTemplate()
    
def generate_message_header(f, xml):

    if xml.little_endian:
        xml.mavlink_endian = "MAVLINK_LITTLE_ENDIAN"
    else:
        xml.mavlink_endian = "MAVLINK_BIG_ENDIAN"

    if xml.crc_extra:
        xml.crc_extra_define = "1"
    else:
        xml.crc_extra_define = "0"

    if xml.sort_fields:
        xml.aligned_fields_define = "1"
    else:
        xml.aligned_fields_define = "0"

    # work out the included headers
    xml.include_list = []
    for i in xml.include:
        base = i[:-4]
        xml.include_list.append(mav_include(base))

    # form message lengths array
    xml.message_lengths_array = ''
    for mlen in xml.message_lengths:
        xml.message_lengths_array += '%u, ' % mlen
    xml.message_lengths_array = xml.message_lengths_array[:-2]

    # and message CRCs array
    xml.message_crcs_array = ''
    for crc in xml.message_crcs:
        xml.message_crcs_array += '%u, ' % crc
    xml.message_crcs_array = xml.message_crcs_array[:-2]

	# and message names array
    xml.message_names_array = ''
    for name in xml.message_names:
        if name is not None:
            xml.message_names_array += '"%s", ' % name.upper()
        else:
            xml.message_names_array += 'null, '
    xml.message_names_array = xml.message_names_array[:-2]

	# and message names enum
    xml.message_names_enum = ''
    count = 0
    for name in xml.message_names:
        if name is not None:
            xml.message_names_enum += '%s = %u,\n' % (name, count)
        count += 1

    # form message info array
    xml.message_info_array = ''
    for name in xml.message_names:
        if name is not None:
            xml.message_info_array += 'typeof( mavlink_%s_t ), ' % name.lower()
        else:
            xml.message_info_array += 'null, '
    xml.message_info_array = xml.message_info_array[:-2]
    

    # add some extra field attributes for convenience with arrays
    for m in xml.enum:
        m.description = m.description.replace("\n"," ")
        m.description = m.description.replace("\r"," ")
        for fe in m.entry:
            fe.description = fe.description.replace("\n"," ")
            fe.description = fe.description.replace("\r"," ")
            fe.name = fe.name.replace(m.name + "_","")
            fe.name = fe.name.replace("NAV_","")
            firstchar = re.search('^([0-9])', fe.name )
            if firstchar != None and firstchar.group():
                fe.name = '_%s' % fe.name
                print(fe.name)
           
    t.write(f, '''
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public partial class MAVLink
{
        public const string MAVLINK_BUILD_DATE = "${parse_time}";
        public const string MAVLINK_WIRE_PROTOCOL_VERSION = "${wire_protocol_version}";
        public const int MAVLINK_MAX_DIALECT_PAYLOAD_SIZE = ${largest_payload};

        public const int MAVLINK_LITTLE_ENDIAN = 1;
        public const int MAVLINK_BIG_ENDIAN = 0;

        public const byte MAVLINK_STX = ${protocol_marker};

        public const byte MAVLINK_ENDIAN = ${mavlink_endian};

        public const bool MAVLINK_ALIGNED_FIELDS = (${aligned_fields_define} == 1);

        public const byte MAVLINK_CRC_EXTRA = ${crc_extra_define};
        
        public const bool MAVLINK_NEED_BYTE_SWAP = (MAVLINK_ENDIAN == MAVLINK_LITTLE_ENDIAN);
        
        public static readonly byte[] MAVLINK_MESSAGE_LENGTHS = new byte[] {${message_lengths_array}};

        public static readonly byte[] MAVLINK_MESSAGE_CRCS = new byte[] {${message_crcs_array}};

        public static readonly Type[] MAVLINK_MESSAGE_INFO = new Type[] {${message_info_array}};

		public static readonly string[] MAVLINK_NAMES = new string[] {${message_names_array}};

        public const byte MAVLINK_VERSION = ${version};

		public enum MAVLINK_MSG_ID 
		{
			${message_names_enum}
		}

    
        ${{enum:
        ///<summary> ${description} </summary>
        public enum ${name}
        {
    ${{entry:	///<summary> ${description} |${{param:${description}| }} </summary>
            ${name}=${value}, 
        }}
        };
        }}
    
''', xml)

def generate_message_enums(f, xml):
    # add some extra field attributes for convenience with arrays
    for m in xml.enum:
        m.description = m.description.replace("\n"," ")
        m.description = m.description.replace("\r"," ")
        for fe in m.entry:
            fe.description = fe.description.replace("\n"," ")
            fe.description = fe.description.replace("\r"," ")
            fe.name = fe.name.replace(m.name + "_","")
            firstchar = re.search('^([0-9])', fe.name )
            if firstchar != None and firstchar.group():
                fe.name = '_%s' % fe.name
                print(fe.name)
            
    t.write(f, '''
        ${{enum:
        ///<summary> ${description} </summary>
        public enum ${name}
        {
    ${{entry:	///<summary> ${description} |${{param:${description}| }} </summary>
            ${name}=${value}, 
        }}
        };
        }}
''', xml)


def generate_message_footer(f, xml):
    t.write(f, '''
}
''', xml)
    f.close()
             

def generate_message_h(f, directory, m):
    '''generate per-message header for a XML file'''
    t.write(f, '''

    [StructLayout(LayoutKind.Sequential,Pack=1,Size=${wire_length})]
    public struct mavlink_${name_lower}_t
    {
${{ordered_fields:        /// <summary> ${description} </summary>
        ${array_prefix} ${type} ${name}${array_suffix};
    }}
    };

''', m)


class mav_include(object):
    def __init__(self, base):
        self.base = base

def generate_one(fh, basename, xml):
    '''generate headers for one XML file'''

    directory = os.path.join(basename, xml.basename)

    print("Generating CSharp implementation in directory %s" % directory)
    mavparse.mkdir_p(directory)

    # add some extra field attributes for convenience with arrays
    for m in xml.message:
        m.msg_name = m.name
        if xml.crc_extra:
            m.crc_extra_arg = ", %s" % m.crc_extra
        else:
            m.crc_extra_arg = ""
        m.msg_nameid = "MAVLINK_MSG_ID_${name} = ${id}"
        for f in m.fields:
            f.description = f.description.replace("\n"," ")
            f.description = f.description.replace("\r","")
            if f.array_length != 0:
                f.array_suffix = ''
                f.array_prefix = '[MarshalAs(UnmanagedType.ByValArray,SizeConst=%u)]\n\t\tpublic' % f.array_length
                f.array_arg = ', %u' % f.array_length
                f.array_return_arg = '%u, ' % (f.array_length)
                f.array_tag = ''
                f.array_const = 'const '
                f.decode_left = "%s.%s = " % (m.name_lower, f.name)
                f.decode_right = ''
                f.return_type = 'void'
                f.return_value = 'void'
                if f.type == 'char': 
                    f.type = "byte[]"
                    f.array_tag = 'System.Text.ASCIIEncoding.ASCII.GetString(msg,%u,%u); //' % (f.wire_offset, f.array_length)
                    f.return_type = 'byte[]'
                    f.c_test_value = ".ToCharArray()";
                elif f.type == 'uint8_t':
                    f.type = "byte[]";
                    f.array_tag = 'getBytes'
                    f.return_type = 'byte[]'
                elif f.type == 'int8_t':
                    f.type = "byte[]";
                    f.array_tag = 'getBytes'
                    f.return_type = 'byte[]'
                elif f.type == 'int16_t':
                    f.type = "Int16[]";
                    f.array_tag = 'getBytes'
                    f.return_type = 'Int16[]'
                elif f.type == 'uint16_t':
                    f.type = "UInt16[]";
                    f.array_tag = 'getBytes'
                    f.return_type = 'UInt16[]'
                else:
                    test_strings = []
                    for v in f.test_value:
                        test_strings.append(str(v))
                    f.c_test_value = '{ %s }' % ', '.join(test_strings)
                    f.array_tag = '!!!%s' % f.type
                f.get_arg = ', %s %s' % (f.type, f.name)
            else:
                if f.type == 'char':
                    f.type = "byte";
                elif f.type == 'uint8_t':
                    f.type = "byte";
                elif f.type == 'int8_t':
                    f.type = "byte";
                elif f.type == 'int16_t': 
                    f.type = "Int16";
                elif f.type == 'uint16_t': 
                    f.type = "UInt16";
                elif f.type == 'uint32_t':
                    f.type = "UInt32";
                elif f.type == 'int16_t': 
                    f.type = "Int16";
                elif f.type == 'int32_t':
                    f.type = "Int32";
                elif f.type == 'uint64_t':
                    f.type = "UInt64";                  
                elif f.type == 'int64_t':     
                    f.type = "Int64";   
                elif f.type == 'float':     
                    f.type = "Single"; 					
                else:
                    f.c_test_value = f.test_value
                f.array_suffix = ''
                f.array_prefix = 'public '
                f.array_tag = 'BitConverter.To%s' % f.type
                if f.type == 'byte':
                    f.array_tag = 'getByte'
                if f.name == 'fixed':   # this is a keyword
                    f.name = '@fixed' 
                f.array_arg = ''
                f.array_return_arg = ''
                f.array_const = ''
                f.decode_left = "%s.%s = " % (m.name_lower, f.name)
                f.decode_right = ''
                f.get_arg = ''
                f.c_test_value = f.test_value
                f.return_type = f.type

    # cope with uint8_t_mavlink_version
    for m in xml.message:
        m.arg_fields = []
        m.array_fields = []
        m.scalar_fields = []
        for f in m.ordered_fields:
            if f.array_length != 0:
                m.array_fields.append(f)
            else:
                m.scalar_fields.append(f)
        for f in m.fields:
            if not f.omit_arg:
                m.arg_fields.append(f)
                f.putname = f.name
            else:
                f.putname = f.const_value
    
    for m in xml.message:
        generate_message_h(fh, directory, m)
        



def generate(basename, xml_list):
    '''generate complete MAVLink C implemenation'''
    
    print ("HERE ",basename, xml_list[0])
    
    directory = os.path.join(basename, xml_list[0].basename)
    
    if not os.path.exists(directory): 
        os.makedirs(directory) 

    f = open(os.path.join(directory, "mavlink.cs"), mode='w')
    
    generate_message_header(f, xml_list[0])
    
    if len(xml_list) > 1:
        generate_message_enums(f, xml_list[1]);
    
    for xml in xml_list:
        generate_one(f, basename, xml)
        
    generate_message_footer(f,xml)
    
