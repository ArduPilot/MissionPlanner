using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MissionPlanner.Utilities
{
    public class OpticalFlow: IDisposable
    {
        // since its not listed in the docs
        // https://github.com/PX4/Flow/blob/4a314cfdb099aed9795b825e7518203771207fbb/src/modules/flow/dcmi.c#L292

        // size of incomming data
        MAVLink.mavlink_data_transmission_handshake_t msgDataTransmissionHandshake;

        // data from handshake
        MAVLink.mavlink_encapsulated_data_t msgEncapsulatedData;

        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> subDataTrans;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> subEncapData;

        MAVLinkInterface _mav;

        MemoryStream imageBuffer = new MemoryStream();

        public event EventHandler<ImageEventHandle> newImage;

        public class ImageEventHandle : EventArgs
        {
            public Image Image { get; set; }

            public ImageEventHandle(Image bmp)
            {
                Image = bmp;
            }
        }

        public OpticalFlow(MAVLinkInterface mav)
        {
            _mav = mav;
             
            subDataTrans = mav.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.DATA_TRANSMISSION_HANDSHAKE, ReceviedPacket);
            subEncapData = mav.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA, ReceviedPacket);
        }

        public void CalibrationMode(bool on_off = false)
        {
            if (on_off == true)
                _mav.setParam("VIDEO_ONLY", 1);
            else
                _mav.setParam("VIDEO_ONLY", 0);
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage arg)
        {
            if (arg.msgid == (byte) MAVLink.MAVLINK_MSG_ID.DATA_TRANSMISSION_HANDSHAKE)
            {
                var packet = arg.ToStructure<MAVLink.mavlink_data_transmission_handshake_t>();
                msgDataTransmissionHandshake = packet;
                imageBuffer.Close();
                imageBuffer = new MemoryStream((int)packet.size);
            }
            else if (arg.msgid == (byte)MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA)
            {
                var packet = arg.ToStructure<MAVLink.mavlink_encapsulated_data_t>();
                msgEncapsulatedData = packet;
                int start = msgDataTransmissionHandshake.payload * msgEncapsulatedData.seqnr;
                int left = (int)msgDataTransmissionHandshake.size - start;
                var writeamount = Math.Min(msgEncapsulatedData.data.Length, left);

                imageBuffer.Seek(start, SeekOrigin.Begin);
                imageBuffer.Write(msgEncapsulatedData.data, 0, writeamount);

                // we have a complete image
                if ((msgEncapsulatedData.seqnr+1) == msgDataTransmissionHandshake.packets)
                {
                    using (
                        Bitmap bmp = new Bitmap(msgDataTransmissionHandshake.width, msgDataTransmissionHandshake.height,
                            PixelFormat.Format8bppIndexed))
                    {
                        SetGrayscalePalette(bmp);

                        var bitmapData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite,
                            bmp.PixelFormat);

                        if (imageBuffer.Length > msgDataTransmissionHandshake.size)
                            return true;

                        var buffer = imageBuffer.ToArray();

                        Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
                        bmp.UnlockBits(bitmapData);

                        if (newImage != null)
                            newImage(this, new ImageEventHandle(bmp));

                        //bmp.Save("test.bmp", ImageFormat.Bmp);
                    }
                }
            }

            return true;
        }

        public void SetGrayscalePalette(Bitmap bmp)
        {
            ColorPalette _palette = bmp.Palette;
            for (int i = 0; i < 256; i++)
                _palette.Entries[i] = Color.FromArgb(255, i, i, i);
            bmp.Palette = _palette;
        }

        public void Close()
        {
            _mav.UnSubscribeToPacketType(subDataTrans);
            _mav.UnSubscribeToPacketType(subEncapData);

            imageBuffer.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
