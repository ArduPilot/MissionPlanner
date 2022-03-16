using System;
using System.Collections.Generic;
using System.IO;

namespace Rtsp
{
    // This class handles the H265 Payload
    // It has methods to parse parameters in the SDP
    // It has methods to process the RTP Payload

    // By Roger Hardiman, RJH Technical Consultancy Ltd

    public class H265Payload
    {
        // H265 / HEVC structure.
        // An 'Access Unit' is the set of NAL Units that form one Picture
        // NAL Units have a 2 byte header comprising of
        // F Bit, Type, Layer ID and TID


        int single, agg, frag = 0; // used for diagnostics stats
        bool has_donl = false;

        List<byte[]> temporary_rtp_payloads = new List<byte[]>(); // used to assemble the RTP packets that form one RTP Frame
                                                                  // Eg all the RTP Packets from M=0 through to M=1

        MemoryStream fragmented_nal = new MemoryStream(); // used to concatenate fragmented H264 NALs where NALs are split over RTP packets


        // Constructor
        public H265Payload(bool has_donl)
        {
            this.has_donl = has_donl;
        }

        public List<byte[]> Process_H265_RTP_Packet(byte[] rtp_payload, int rtp_marker) {

            // Add payload to the List of payloads for the current Frame of Video
            // ie all the payloads with M=0 up to the final payload where M=1
            temporary_rtp_payloads.Add(rtp_payload); // Todo Could optimise this and go direct to Process Frame if just 1 packet in frame

            if (rtp_marker == 1)
            {
                // End Marker is set. Process the list of RTP Packets (forming 1 RTP frame) and save the NALs to a file
                List<byte[]> nal_units = Process_H265_RTP_Frame(temporary_rtp_payloads);
                temporary_rtp_payloads.Clear();

                return nal_units;
            }

            return null; // we don't have a frame yet. Keep accumulating RTP packets
        }


        // Process a RTP Frame. A RTP Frame can consist of several RTP Packets which have the same Timestamp
        // Returns a list of NAL Units (with no 00 00 00 01 header and with no Size header)
        private List<byte[]> Process_H265_RTP_Frame(List<byte[]> rtp_payloads)
        {
            Console.WriteLine("RTP Data comprised of " + rtp_payloads.Count + " rtp packets");

            List<byte[]> nal_units = new List<byte[]>(); // Stores the NAL units for a Video Frame. May be more than one NAL unit in a video frame.

            for (int payload_index = 0; payload_index < rtp_payloads.Count; payload_index++)
            {
                // Examine the first two bytes of the RTP data, the Payload Header
                // F (Forbidden Bit),
                // Type of NAL Unit (or VCL NAL Unit if Type is < 32),
                // LayerId
                // TID  (TemporalID = TID - 1)
                /*+---------------+---------------+
                 *|0|1|2|3|4|5|6|7|0|1|2|3|4|5|6|7|
                 *+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                 *|F|   Type    |  LayerId  | TID |
                 *+-------------+-----------------+
                 */

                int payload_header = (rtp_payloads[payload_index][0] << 8) | (rtp_payloads[payload_index][1]);
                int payload_header_f_bit = (payload_header >> 15) & 0x01;
                int payload_header_type = (payload_header >> 9) & 0x3F;
                int payload_header_layer_id = (payload_header >> 3) & 0x3F;
                int payload_header_tid = payload_header & 0x7;


                // There are three ways to Packetize NAL units into RTP Packets
                //  Single NAL Unit Packet
                //  Aggregation Packet (payload_header_type = 48)
                //  Fragmentation Unit (payload_header_type = 49)


                // Single NAL Unit Packet
                // 32=VPS
                // 33=SPS
                // 34=PPS
                if (payload_header_type != 48 && payload_header_type != 49)
                {
                    Console.WriteLine("Single NAL");
                    single++;

                    //TODO - Handle DONL

                    nal_units.Add(rtp_payloads[payload_index]);
                }

                // Aggregation Packet
                else if (payload_header_type == 48)
                {
                    Console.WriteLine("Aggregation Packet");
                    agg++;

                    // RTP packet contains multiple NALs, each with a 16 bit header
                    //   Read 16 byte size
                    //   Read NAL
                    // Use a Try/Catch to protect from bad RTP data where block sizes exceed the
                    // available data
                    try
                    {
                        int ptr = 2; // start after 16 bit Payload Header

                        // loop until the ptr has moved beyond the length of the data
                        while (ptr < (rtp_payloads[payload_index].Length - 1))
                        {
                            if (has_donl) ptr = ptr + 2; // step over the DONL data
                            int size = (rtp_payloads[payload_index][ptr] << 8) + (rtp_payloads[payload_index][ptr + 1] << 0);
                            ptr = ptr + 2;
                            byte[] nal = new byte[size];
                            System.Array.Copy(rtp_payloads[payload_index], ptr, nal, 0, size); // copy the NAL
                            nal_units.Add(nal); // Add to list of NALs for this RTP frame. Start Codes like 00 00 00 01 get added later
                            ptr = ptr + size;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("H265 Aggregate Packet processing error");
                    }
                }

                // Fragmentation Unit
                else if (payload_header_type == 49)
                {
                    Console.WriteLine("Fragmentation Unit");
                    frag++;

                    // Parse Fragmentation Unit Header
                    int fu_header_s = (rtp_payloads[payload_index][2] >> 7) & 0x01;  // start marker
                    int fu_header_e = (rtp_payloads[payload_index][2] >> 6) & 0x01;  // end marker
                    int fu_header_type = (rtp_payloads[payload_index][2] >> 0) & 0x3F; // fu type

                    Console.WriteLine("Frag FU-A s=" + fu_header_s + "e=" + fu_header_e);

                    // Check Start and End flags
                    if (fu_header_s == 1 && fu_header_e == 0)
                    {
                        // Start of Fragment.
                        // Initiise the fragmented_nal byte array

                        // Empty the stream
                        fragmented_nal.SetLength(0);

                        // Reconstrut the NAL header from the rtp_payload_header, replacing the Type with FU Type
                        int nal_header = (payload_header & 0x81FF); // strip out existing 'type'
                        nal_header = nal_header | (fu_header_type << 9);

                        fragmented_nal.WriteByte((byte)((nal_header >> 8) & 0xFF));
                        fragmented_nal.WriteByte((byte)((nal_header >> 0) & 0xFF));

                        if (has_donl)
                        {
                            // start copying after the DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 5, rtp_payloads[payload_index].Length - 5);
                        }
                        else
                        {
                            // there is no DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 3, rtp_payloads[payload_index].Length - 3);
                        }
                    }

                    if (fu_header_s == 0 && fu_header_e == 0)
                    {
                        // Middle part of Fragment
                        // Append this payload to the fragmented_nal

                        if (has_donl) {
                            // start copying after the DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 5, rtp_payloads[payload_index].Length - 5);
                        } else {
                            // there is no DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 3, rtp_payloads[payload_index].Length - 3);
                        }
                    }

                    if (fu_header_s == 0 && fu_header_e == 1)
                    {
                        // End part of Fragment
                        // Append this payload to the fragmented_nal
                        if (has_donl)
                        {
                            // start copying after the DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 5, rtp_payloads[payload_index].Length - 5);
                        }
                        else
                        {
                            // there is no DONL data
                            fragmented_nal.Write(rtp_payloads[payload_index], 3, rtp_payloads[payload_index].Length - 3);
                        }

                        // Add the NAL to the array of NAL units
                        nal_units.Add(fragmented_nal.ToArray());
                    }
                }
                else {
                    Console.WriteLine("Unknown Payload Header Type = " + payload_header_type);
                }
            }

            // Output some statistics
            Console.WriteLine("Single=" + single + " Agg=" + agg + " Frag=" + frag);

            // Output all the NALs that form one RTP Frame (one frame of video)
            return nal_units;

        }
    }
}
