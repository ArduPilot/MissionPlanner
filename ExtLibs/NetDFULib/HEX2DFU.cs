using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetDFULib
{
    public class HEX2DFU
    {
        const uint STDFUFILES_ERROR_OFFSET = 0x12346000; // File access error offset.
        const uint STDFUFILES_NOERROR = 0x12340000;      // No error.
        const uint STDFUFILES_BADSUFFIX = 0x12346002;    // Bad DFU Suffix format.
        const uint STDFUFILES_UNABLETOOPENFILE = 0x12346003; // Unable to open file.
        const uint STDFUFILES_UNABLETOOPENTEMPFILE = 0x12346004; // Unable to open temporary file.
        const uint STDFUFILES_BADFORMAT = 0x12346005;           // Bad DFU file format.
        const uint STDFUFILES_BADADDRESSRANGE = 0x12346006; // Bad address range.
        const uint STDFUFILES_BADPARAMETER = 0x12346008; // Bad parameter.
        const uint STDFUFILES_UNEXPECTEDERROR = 0x1234600A; // Unexpected error.
        const uint STDFUFILES_FILEGENERALERROR = 0x1234600D; // File general error

        public bool ConvertHexToDFU(string hex_file_name, string dfu_output_name, UInt16 Vid, UInt16 Pid, UInt16 Bcd)
        {
            IntPtr dfu_handle = (IntPtr)0;
            Boolean image_valid = false;
            bool success = false;
            uint status = STDFUFILES_CreateNewDFUFile(dfu_output_name, ref dfu_handle, Vid, Pid, Bcd);
            if (STDFUFILES_NOERROR == status)
            {
                IntPtr image_handle = (IntPtr)0;
                byte alternate = 0;
                status = STDFUFILES_ImageFromFile(hex_file_name, ref image_handle, alternate);
                if (STDFUFILES_NOERROR == status)
                {
                    image_valid = true;
                    status = STDFUFILES_AppendImageToDFUFile(dfu_handle, image_handle);
                    if (STDFUFILES_NOERROR == status)
                    {
                        success = true;
                    }
                }
                STDFUFILES_CloseDFUFile(dfu_handle);
                if (image_valid)
                {
                    STDFUFILES_DestroyImage(ref image_handle);
                }
            }
            return success;
        }

        [DllImport("STDFUFiles.DLL", EntryPoint = "STDFUFILES_AppendImageToDFUFile", CharSet = CharSet.Auto)]
        public static extern UInt32 STDFUFILES_AppendImageToDFUFile(IntPtr handle, IntPtr image);

        [DllImport("STDFUFiles.DLL", EntryPoint = "STDFUFILES_CloseDFUFile", CharSet = CharSet.Auto)]
        public static extern UInt32 STDFUFILES_CloseDFUFile(IntPtr handle);

        [DllImport("STDFUFiles.DLL", EntryPoint = "STDFUFILES_CreateNewDFUFile", CharSet = CharSet.Auto)]
        public static extern UInt32 STDFUFILES_CreateNewDFUFile([MarshalAs(UnmanagedType.LPStr)]String szDevicePath, ref IntPtr handle, UInt16 Vid, UInt16 Pid, UInt16 Bcd);

        [DllImport("STDFUFiles.DLL", EntryPoint = "STDFUFILES_DestroyImage", CharSet = CharSet.Auto)]
        public static extern UInt32 STDFUFILES_DestroyImage(ref IntPtr handle);

        [DllImport("STDFUFiles.DLL", EntryPoint = "STDFUFILES_ImageFromFile", CharSet = CharSet.Auto)]
        public static extern UInt32 STDFUFILES_ImageFromFile([MarshalAs(UnmanagedType.LPStr)]String szDevicePath, ref IntPtr image, byte nAlternate);
    }
}
