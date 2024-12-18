import dronecan.dsdl
import argparse
import os
import em
import json
from canard_dsdlc_helpers import *
from multiprocessing import Pool

templates = [
    {'source_file': 'request.h',
     'output_file': 'include/@(msg_header_name_request(msg))'},
    {'source_file': 'response.h',
     'output_file': 'include/@(msg_header_name_response(msg))'},
    {'source_file': 'broadcast.h',
     'output_file': 'include/@(msg_header_name(msg))'},
    {'source_file': 'service.h',
     'output_file': 'include/@(msg_header_name(msg))'},
    {'source_file': 'request.c',
     'output_file': 'src/@(msg_c_file_name_request(msg))'},
    {'source_file': 'response.c',
     'output_file': 'src/@(msg_c_file_name_response(msg))'},
    {'source_file': 'broadcast.c',
     'output_file': 'src/@(msg_c_file_name(msg))'},
]

parser = argparse.ArgumentParser()
parser.add_argument('namespace_dir', nargs='+')
parser.add_argument('build_dir', nargs=1)
parser.add_argument('--build', action='append')
args = parser.parse_args()

buildlist = None

message_names_enum = ''

if args.build:
    buildlist = set(args.build)

namespace_paths = [os.path.abspath(path) for path in args.namespace_dir]
build_dir = os.path.abspath(args.build_dir[0])

print("namespace_paths",namespace_paths)
print("build_dir",build_dir)
print("buildlist",buildlist)

os.chdir(os.path.dirname(__file__))
templates_dir = 'templates'

messages = dronecan.dsdl.parse_namespaces(namespace_paths)
message_dict = {}
for msg in messages:
    #print(msg)
    message_dict[msg.full_name] = msg

for template in templates:
    with open(os.path.join(templates_dir, template['source_file']), 'r') as f:
        template['source'] = f.read()

def build_message(msg_name):
    print ('building %s' % (msg_name))
    print(f'{os.getpid()}: {os.getcwd()}')
    msglocal = message_dict[msg_name]
    #with open('%s.json' % (msg_name), 'w') as f:
    #    f.write(json.dumps(msg, default=lambda x: x.__dict__))
    for template in templates:
        output = em.expand(template['source'], msg=msglocal)

        if not output.strip():
            continue

        
        output_file = os.path.join(build_dir, em.expand('@{from canard_dsdlc_helpers import *}'+template['output_file'], msg=msglocal))
        mkdir_p(os.path.dirname(output_file))
        print(f'Doing {msg_name} {template['source_file']} {output_file} ')
        with open(output_file, 'w') as f:
            f.write(output)
    print(f'Done {msg_name}')

# error callback function
def handler(error):
    print(error)
    raise Exception('Something bad happened')

if __name__ == '__main__':
    print("start main")

    pool = Pool()
    print("pool: buildlist is None")
    msglist = [msg.full_name for msg in messages]
    #for msg_name in [msg.full_name for msg in messages]:
    #    build_message(msg_name)

    pool.map_async(build_message, msglist ,error_callback=handler)

    pool.close()
    pool.join()
    print("pool: Done") 

    for msg_name in  msglist:
        msg = message_dict[msg_name]
        #print (dir(msg))
        if not msg.default_dtid is None and msg.kind == msg.KIND_MESSAGE:
            message_names_enum += '\t(typeof(%s), %s, 0x%08X, (b,s,fd) => %s.ByteArrayToDroneCANMsg(b,s,fd)),\n' % (msg.full_name.replace('.','_'), msg.default_dtid, msg.get_data_type_signature(),msg.full_name.replace('.','_'))

        if not msg.default_dtid is None and msg.kind == msg.KIND_SERVICE:
            message_names_enum += '\t(typeof(%s_req), %s, 0x%08X, (b,s,fd) => %s_req.ByteArrayToDroneCANMsg(b,s,fd)),\n' % (msg.full_name.replace('.','_'), msg.default_dtid, msg.get_data_type_signature(),msg.full_name.replace('.','_'))
            message_names_enum += '\t(typeof(%s_res), %s, 0x%08X, (b,s,fd) => %s_res.ByteArrayToDroneCANMsg(b,s,fd)),\n' % (msg.full_name.replace('.','_'), msg.default_dtid, msg.get_data_type_signature(),msg.full_name.replace('.','_'))


    assert buildlist is None or not buildlist-builtlist, "%s not built" % (buildlist-builtlist,)

    print ('test')
    with open('messages.cs', 'w') as f:
        f.write('using System;using System.Reflection;\nnamespace DroneCAN {\npublic partial class DroneCAN {\n    public static (Type type,UInt16 msgid, ulong crcseed, Func<Byte[],int, bool, object> convert)[] MSG_INFO = { \n%s};}}' % (message_names_enum))

    #print (message_names_enum)

