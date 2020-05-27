using System;
using System.Collections.Generic;
using System.IO;

namespace Rtsp
{
    // This class handles the G711 Payload
    // It has methods to process the RTP Payload

    public class G711Payload
    {
        // Constructor
        public G711Payload()
        {
        }

        public List<byte[]> Process_G711_RTP_Packet(byte[] rtp_payload, int rtp_marker) {
            
            List<byte[]> audio_data = new List<byte[]>();
            audio_data.Add(rtp_payload);

            return audio_data;
        }

        /* Untested - used with G711.1 and PCMA-WB and PCMU-WB Codec Names */
        public List<byte[]> Process_G711_1_RTP_Packet(byte[] rtp_payload, int rtp_marker) {

            // Look at the Header. This tells us the G711 mode being used

            // Mode Index (MI) is
            // 1 - R1 40 octets containg Layer 0 data
            // 2 - R2a 50 octets containing Layer 0 plus Layer 1 data
            // 3 - R2b 50 octets containing Layer 0 plus Layer 2 data
            // 4 - R3 60 octets containing Layer 0 plus Layer 1 plus Layer 2 data

            byte mode_index = (byte)(rtp_payload[0] & 0x07);

            int size_of_one_frame = 0; // will be in bytes
            switch (mode_index) {
                case 1: size_of_one_frame = 40; break;
                case 2: size_of_one_frame = 50; break;
                case 3: size_of_one_frame = 50; break;
                case 4: size_of_one_frame = 60; break;
                default: return null; // invalid Mode Index
            }

            int number_frames = (rtp_payload.Length - 1) / size_of_one_frame;


            // Return just the basic u-Law or A-Law audio (the Layer 0 audio)

            List<byte[]> audio_data = new List<byte[]>();

            // Extract each audio frame and place in the audio_data List
            int frame_start = 1; // starts just after the MI header
            while (frame_start + size_of_one_frame < rtp_payload.Length) {
                byte[] layer_0_audio = new byte[40];
                System.Array.Copy(rtp_payload,frame_start,layer_0_audio,0,40); // 40 octets in Layer 0 data
                audio_data.Add(layer_0_audio);

                frame_start += size_of_one_frame;
            }
            return audio_data;
        }
    }
}
