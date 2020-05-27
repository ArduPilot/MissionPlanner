using System;
using System.Collections.Generic;
using System.IO;

namespace Rtsp
{
    // This class handles the AAC-hbd (High Bitrate) Payload
    // It has methods to process the RTP Payload

    // (c) 2018 Roger Hardiman, RJH Technical Consultancy Ltd


    /*
    RFC 3640
    3.3.6.  High Bit-rate AAC

    This mode is signaled by mode=AAC-hbr.This mode supports the
    transportation of variable size AAC frames.In one RTP packet,
    either one or more complete AAC frames are carried, or a single
    fragment of an AAC frame is carried.In this mode, the AAC frames
    are allowed to be interleaved and hence receivers MUST support de-
    interleaving.The maximum size of an AAC frame in this mode is 8191
    octets.

    In this mode, the RTP payload consists of the AU Header Section,
    followed by either one AAC frame, several concatenated AAC frames or
    one fragmented AAC frame.The Auxiliary Section MUST be empty.  For
    each AAC frame contained in the payload, there MUST be an AU-header
    in the AU Header Section to provide:

    a) the size of each AAC frame in the payload and

    b) index information for computing the sequence(and hence timing) of
       each AAC frame.

       To code the maximum size of an AAC frame requires 13 bits.
       Therefore, in this configuration 13 bits are allocated to the AU-
       size, and 3 bits to the AU-Index(-delta) field.Thus, each AU-header
       has a size of 2 octets.Each AU-Index field MUST be coded with the
       value 0.  In the AU Header Section, the concatenated AU-headers MUST
       be preceded by the 16-bit AU-headers-length field, as specified in
       section 3.2.1.

        In addition to the required MIME format parameters, the following
        parameters MUST be present: sizeLength, indexLength, and
        indexDeltaLength.AAC frames always have a fixed duration per Access
        Unit; when interleaving in this mode, this specific duration MUST be
        signaled by the MIME format parameter constantDuration.In addition,
        the parameter maxDisplacement MUST be present when interleaving.

    For example:

        m= audio 49230 RTP/AVP 96
        a= rtpmap:96 mpeg4-generic/48000/6
        a= fmtp:96 streamtype= 5; profile-level-id= 16; mode= AAC-hbr;config= 11B0; sizeLength= 13; indexLength= 3;indexDeltaLength= 3; constantDuration= 1024

    The hexadecimal value of the "config" parameter is the AudioSpecificConfig(), as defined in ISO/IEC 14496-3.
    AudioSpecificConfig() specifies a 5.1 channel AAC stream with a sampling rate of 48 kHz.For the description of MIME parameters, see
    section 4.1.

    */


    public class AACPayload
    {
        public uint ObjectType = 0;
        public uint FrequencyIndex = 0;
        public uint ChannelConfiguration = 0;

        // Constructor
        public AACPayload(String config_string)
        {
            /***
            5 bits: object type
                if (object type == 31)
                6 bits + 32: object type
            4 bits: frequency index
                if (frequency index == 15)
                24 bits: frequency
            4 bits: channel configuration
            var bits: AOT Specific Config
             ***/

            // config is a string in hex eg 1490 or 0x1210
            // Read each ASCII character and add to a bit array
            BitStream bs = new BitStream();
            bs.AddHexString(config_string);

            // Read 5 bits
            ObjectType = bs.Read(5);

            // Read 4 bits
            FrequencyIndex = bs.Read(4);

            // Read 4 bits
            ChannelConfiguration = bs.Read(4);
        }

        public List<byte[]> Process_AAC_RTP_Packet(byte[] rtp_payload, int rtp_marker) {

            // RTP Payload for MPEG4-GENERIC can consist of multple blocks.
            // Each block has 3 parts
            // Part 1 - Acesss Unit Header Length + Header
            // Part 2 - Access Unit Auxiliary Data Length + Data (not used in AAC High Bitrate)
            // Part 3 - Access Unit Audio Data

            // The rest of the RTP packet is the AMR data
            List<byte[]> audio_data = new List<byte[]>();

            int ptr = 0;

            while (true) {
                if (ptr + 4 > rtp_payload.Length) break; // 2 bytes for AU Header Length, 2 bytes of AU Header payload

                // Get Size of the AU Header
                int au_headers_length_bits = (((rtp_payload[ptr] << 8) + (rtp_payload[ptr + 1] << 0))); // 16 bits
                int au_headers_length = (int)Math.Ceiling((double)au_headers_length_bits / 8.0);
                ptr += 2;

                // Examine the AU Header. Get the size of the AAC data
                int aac_frame_size = (((rtp_payload[ptr] << 8) + (rtp_payload[ptr+1] << 0)) >> 3); // 13 bits
                int aac_index_delta = rtp_payload[ptr+1] & 0x03; // 3 bits
                ptr += au_headers_length;

                // extract the AAC block
                if (ptr + aac_frame_size > rtp_payload.Length) break; // not enough data to copy
                byte[] aac_data = new byte[aac_frame_size];
                System.Array.Copy(rtp_payload, ptr, aac_data, 0, aac_frame_size);
                audio_data.Add(aac_data);
                ptr += aac_frame_size;
            }

            return audio_data;
        }

    }
}
