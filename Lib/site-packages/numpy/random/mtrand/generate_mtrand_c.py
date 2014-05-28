import re
import subprocess


def remove_long_path():
    path = 'mtrand.c'
    pat = re.compile(r'"[^"]*mtrand\.pyx"')
    code = open(path).read()
    code = pat.sub(r'"mtrand.pyx"', code)
    open(path, 'w').write(code)


def main():
    subprocess.check_call(['cython', 'mtrand.pyx'])
    remove_long_path()


if __name__ == '__main__':
    main()
