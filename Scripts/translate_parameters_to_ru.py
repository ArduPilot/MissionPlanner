import sys
import os
import time
import xml.etree.ElementTree as ET

# Avoid importing helper scripts in this directory that shadow standard modules
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
if SCRIPT_DIR in sys.path:
    sys.path.remove(SCRIPT_DIR)

from googletrans import Translator


def is_cyrillic(text: str) -> bool:
    return any('А' <= ch <= 'я' or ch in 'Ёё' for ch in text)


def translate_file(src: str, dst: str) -> None:
    tree = ET.parse(src)
    root = tree.getroot()
    translator = Translator()
    # Force library to raise exceptions on errors to avoid silent failures
    if not hasattr(translator, 'raise_Exception'):
        # Newer googletrans versions renamed the attribute, keep compatibility
        setattr(translator, 'raise_Exception', True)
    else:
        translator.raise_Exception = True

    # Standard ParameterFactMetaData format
    count = 0
    for param in root.findall('.//parameter'):
        for tag in ['short_desc', 'long_desc']:
            elem = param.find(tag)
            if elem is not None and elem.text and not is_cyrillic(elem.text):
                try:
                    elem.text = translator.translate(elem.text, dest='ru').text
                    count += 1
                    if count % 50 == 0:
                        time.sleep(0.5)
                except Exception as e:
                    print(f'Failed to translate "{elem.text}": {e}', file=sys.stderr)

    # PX4 ParameterMetaDataBackup style
    if root.tag == 'Params':
        for group in root:
            for param in group:
                for tag in ['DisplayName', 'Description']:
                    elem = param.find(tag)
                    if elem is not None and elem.text and not is_cyrillic(elem.text):
                        try:
                            elem.text = translator.translate(elem.text, dest='ru').text
                            count += 1
                            if count % 50 == 0:
                                time.sleep(0.5)
                        except Exception as e:
                            print(f'Failed to translate "{elem.text}": {e}', file=sys.stderr)
    tree.write(dst, encoding='utf-8', xml_declaration=True)


if __name__ == '__main__':
    if len(sys.argv) != 3:
        print('Usage: python translate_parameters_to_ru.py <source_xml> <dest_xml>')
        sys.exit(1)
    translate_file(sys.argv[1], sys.argv[2])
