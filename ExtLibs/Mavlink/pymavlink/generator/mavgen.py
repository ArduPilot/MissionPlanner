#!/usr/bin/env python

'''
parse a MAVLink protocol XML file and generate a python implementation

Copyright Andrew Tridgell 2011
Released under GNU GPL version 3 or later

'''
import sys, textwrap, os, copy
from . import mavparse

# XSD schema file
schemaFile = os.path.join(os.path.dirname(os.path.realpath(__file__)), "mavschema.xsd")

# Set defaults for generating MAVLink code
DEFAULT_WIRE_PROTOCOL = mavparse.PROTOCOL_1_0
DEFAULT_LANGUAGE = 'Python'
DEFAULT_ERROR_LIMIT = 200
DEFAULT_VALIDATE = True

# List the supported languages. This is done globally because it's used by the GUI wrapper too
supportedLanguages = ["C", "CS", "csharp", "JavaScript", "Python", "WLua", "ObjC", "Swift", "Java"]


def mavgen(opts, args) :
    """Generate mavlink message formatters and parsers (C and Python ) using options
    and args where args are a list of xml files. This function allows python
    scripts under Windows to control mavgen using the same interface as
    shell scripts under Unix"""

    xml = []

    # Enable validation by default, disabling it if explicitly requested
    if opts.validate:
        try:
            from lib.genxmlif import GenXmlIfError
            from lib.minixsv import pyxsval
        except:
            print("WARNING: Unable to load XML validator libraries. XML validation will not be performed")
            opts.validate = False

    def mavgen_validate(fname, schema, errorLimitNumber) :
        """Uses minixsv to validate an XML file with a given XSD schema file. We define mavgen_validate
           here because it relies on the XML libs that were loaded in mavgen(), so it can't be called standalone"""
        # use default values of minixsv, location of the schema file must be specified in the XML file
        domTreeWrapper = pyxsval.parseAndValidate(fname, xsdFile=schema, errorLimit=errorLimitNumber)

    # Process all XML files, validating them as necessary.
    for fname in args:
        if opts.validate:
            print("Validating %s" % fname)
            mavgen_validate(fname, schemaFile, opts.error_limit);
        else:
            print("Validation skipped for %s." % fname)

        print("Parsing %s" % fname)
        xml.append(mavparse.MAVXML(fname, opts.wire_protocol))

    # expand includes
    for x in xml[:]:
        for i in x.include:
            fname = os.path.join(os.path.dirname(x.filename), i)

            ## Validate XML file with XSD file if possible.
            if opts.validate:
                print("Validating %s" % fname)
                mavgen_validate(fname, schemaFile, opts.error_limit);
            else:
                print("Validation skipped for %s." % fname)

            ## Parsing
            print("Parsing %s" % fname)
            xml.append(mavparse.MAVXML(fname, opts.wire_protocol))

            # include message lengths and CRCs too
            x.message_crcs.update(xml[-1].message_crcs)
            x.message_lengths.update(xml[-1].message_lengths)
            x.message_min_lengths.update(xml[-1].message_min_lengths)
            x.message_flags.update(xml[-1].message_flags)
            x.message_target_system_ofs.update(xml[-1].message_target_system_ofs)
            x.message_target_component_ofs.update(xml[-1].message_target_component_ofs)
            x.message_names.update(xml[-1].message_names)
            x.largest_payload = max(x.largest_payload, xml[-1].largest_payload)

    # work out max payload size across all includes
    largest_payload = 0
    for x in xml:
        if x.largest_payload > largest_payload:
            largest_payload = x.largest_payload
    for x in xml:
        x.largest_payload = largest_payload

    if mavparse.check_duplicates(xml):
        sys.exit(1)

    print("Found %u MAVLink message types in %u XML files" % (
        mavparse.total_msgs(xml), len(xml)))

    # Convert language option to lowercase and validate
    opts.language = opts.language.lower()
    if opts.language == 'python':
        from . import mavgen_python
        mavgen_python.generate(opts.output, xml)
    elif opts.language == 'c':
        from . import mavgen_c
        mavgen_c.generate(opts.output, xml)
    elif opts.language == 'wlua':
        from . import mavgen_wlua
        mavgen_wlua.generate(opts.output, xml)
    elif opts.language == 'cs':
        from . import mavgen_cs
        mavgen_cs.generate(opts.output, xml)
    elif opts.language == 'csharp':
        from . import mavgen_csharp
        mavgen_csharp.generate(opts.output, xml)
    elif opts.language == 'javascript':
        from . import mavgen_javascript
        mavgen_javascript.generate(opts.output, xml)
    elif opts.language == 'objc':
        from . import mavgen_objc
        mavgen_objc.generate(opts.output, xml)
    elif opts.language == 'swift':
        from . import mavgen_swift
        mavgen_swift.generate(opts.output, xml)
    elif opts.language == 'java':
        from . import mavgen_java
        mavgen_java.generate(opts.output, xml)
    else:
        print("Unsupported language %s" % opts.language)


# build all the dialects in the dialects subpackage
class Opts:
    def __init__(self, output, wire_protocol=DEFAULT_WIRE_PROTOCOL, language=DEFAULT_LANGUAGE, validate=DEFAULT_VALIDATE, error_limit=DEFAULT_ERROR_LIMIT):
        self.wire_protocol = wire_protocol
        self.error_limit = error_limit
        self.language = language
        self.output = output
        self.validate = validate

def mavgen_python_dialect(dialect, wire_protocol):
    '''generate the python code on the fly for a MAVLink dialect'''
    dialects = os.path.join(os.path.dirname(os.path.realpath(__file__)), '..', 'dialects')
    mdef = os.path.join(os.path.dirname(os.path.realpath(__file__)), '..', '..', 'message_definitions')
    if wire_protocol == mavparse.PROTOCOL_0_9:
        py = os.path.join(dialects, 'v09', dialect + '.py')
        xml = os.path.join(dialects, 'v09', dialect + '.xml')
        if not os.path.exists(xml):
            xml = os.path.join(mdef, 'v0.9', dialect + '.xml')
    elif wire_protocol == mavparse.PROTOCOL_1_0:
        py = os.path.join(dialects, 'v10', dialect + '.py')
        xml = os.path.join(dialects, 'v10', dialect + '.xml')
        if not os.path.exists(xml):
            xml = os.path.join(mdef, 'v1.0', dialect + '.xml')
    else:
        py = os.path.join(dialects, 'v20', dialect + '.py')
        xml = os.path.join(dialects, 'v20', dialect + '.xml')
        if not os.path.exists(xml):
            xml = os.path.join(mdef, 'v1.0', dialect + '.xml')
    opts = Opts(py, wire_protocol)

     # Python 2 to 3 compatibility
    try:
        import StringIO as io
    except ImportError:
        import io

    # throw away stdout while generating
    stdout_saved = sys.stdout
    sys.stdout = io.StringIO()
    try:
        xml = os.path.relpath(xml)
        mavgen( opts, [xml] )
    except Exception:
        sys.stdout = stdout_saved
        raise
    sys.stdout = stdout_saved

if __name__ == "__main__":
    raise DeprecationWarning("Executable was moved to pymavlink.tools.mavgen")
