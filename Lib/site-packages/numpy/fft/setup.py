from numpy.distutils.system_info import get_info


def configuration(parent_package='',top_path=None):
    from numpy.distutils.misc_util import Configuration
    config = Configuration('fft',parent_package,top_path)

    config.add_data_dir('tests')

    # Configure fftpack_lite
    config.add_extension('fftpack_lite',
                         sources=['fftpack_litemodule.c', 'fftpack.c'],
                         **get_info('ndarray'))

    return config


if __name__ == '__main__':
    from numpy.distutils.core import setup
    setup(configuration=configuration)
