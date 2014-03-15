from os.path import join

from numpy.distutils.system_info import get_info


def configuration(parent_package='',top_path=None):
    from numpy.distutils.misc_util import Configuration
    config = Configuration('numarray', parent_package,top_path)

    config.add_data_files('include/numpy/*')

    config.add_extension('_capi',
                         sources=['_capi.c'],
                         **get_info('ndarray'))

    return config


if __name__ == '__main__':
    from numpy.distutils.core import setup
    setup(configuration=configuration)
