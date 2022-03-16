using System;
using System.Collections.Generic;
using System.IO;

namespace Rtsp
{
    // This class handles the H264 Payload
    // It has methods to parse parameters in the SDP
    // It has methods to process the RTP Payload

    public class H264Payload
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        int norm, fu_a, fu_b, stap_a, stap_b, mtap16, mtap24 = 0; // used for diagnostics stats

        List<byte[]> temporary_rtp_payloads = new List<byte[]>(); // used to assemble the RTP packets that form one RTP Frame
                                                                  // Eg all the RTP Packets from M=0 through to M=1

        MemoryStream fragmented_nal = new MemoryStream(); // used to concatenate fragmented H264 NALs where NALs are split over RTP packets


        // Constructor
        public H264Payload()
        {
        }

        public List<byte[]> Process_H264_RTP_Packet(byte[] rtp_payload, int rtp_marker) {

            // Add to the list of payloads for the current Frame of video
            temporary_rtp_payloads.Add(rtp_payload); // Todo Could optimise this and go direct to Process Frame if just 1 packet in frame

            if (rtp_marker == 1)
            {
                // End Marker is set. Process the list of RTP Packets (forming 1 RTP frame) and save the NALs to a file
                List<byte[]> nal_units = Process_H264_RTP_Frame(temporary_rtp_payloads);
                temporary_rtp_payloads.Clear();

                return nal_units;
            }

            return null; // we don't have a frame yet. Keep accumulating RTP packets
        }


        // Process a RTP Frame. A RTP Frame can consist of several RTP Packets which have the same Timestamp
        // Returns a list of NAL Units (with no 00 00 00 01 header and with no Size header)
        private List<byte[]> Process_H264_RTP_Frame(List<byte[]> rtp_payloads)
        {
            _logger.Debug("RTP Data comprised of " + rtp_payloads.Count + " rtp packets");

            List<byte[]> nal_units = new List<byte[]>(); // Stores the NAL units for a Video Frame. May be more than one NAL unit in a video frame.

            for (int payload_index = 0; payload_index < rtp_payloads.Count; payload_index++)
            {
                // Examine the first rtp_payload and the first byte (the NAL header)
                int nal_header_f_bit = (rtp_payloads[payload_index][0] >> 7) & 0x01;
                int nal_header_nri = (rtp_payloads[payload_index][0] >> 5) & 0x03;
                int nal_header_type = (rtp_payloads[payload_index][0] >> 0) & 0x1F;

                // If the Nal Header Type is in the range 1..23 this is a normal NAL (not fragmented)
                // So write the NAL to the file
                if (nal_header_type >= 1 && nal_header_type <= 23)
                {
                    _logger.Debug("Normal NAL");
                    norm++;
                    nal_units.Add(rtp_payloads[payload_index]);
                }
                // There are 4 types of Aggregation Packet (split over RTP payloads)
                else if (nal_header_type == 24)
                {
                    _logger.Debug("Agg STAP-A");
                    stap_a++;

                    // RTP packet contains multiple NALs, each with a 16 bit header
                    //   Read 16 byte size
                    //   Read NAL
                    try
                    {
                        int ptr = 1; // start after the nal_header_type which was '24'
                        // if we have at least 2 more bytes (the 16 bit size) then consume more data
                        while (ptr + 2 < (rtp_payloads[payload_index].Length - 1))
                        {
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
                        _logger.Debug("H264 Aggregate Packet processing error");
                    }
                }
                else if (nal_header_type == 25)
                {
                    _logger.Debug("Agg STAP-B not supported");
                    stap_b++;
                }
                else if (nal_header_type == 26)
                {
                    _logger.Debug("Agg MTAP16 not supported");
                    mtap16++;
                }
                else if (nal_header_type == 27)
                {
                    _logger.Debug("Agg MTAP24 not supported");
                    mtap24++;
                }
                else if (nal_header_type == 28)
                {
                    _logger.Debug("Frag FU-A");
                    fu_a++;

                    // Parse Fragmentation Unit Header
                    int fu_header_s = (rtp_payloads[payload_index][1] >> 7) & 0x01;  // start marker
                    int fu_header_e = (rtp_payloads[payload_index][1] >> 6) & 0x01;  // end marker
                    int fu_header_r = (rtp_payloads[payload_index][1] >> 5) & 0x01;  // reserved. should be 0
                    int fu_header_type = (rtp_payloads[payload_index][1] >> 0) & 0x1F; // Original NAL unit header

                    _logger.Debug("Frag FU-A s=" + fu_header_s + "e=" + fu_header_e);

                    // Check Start and End flags
                    if (fu_header_s == 1 && fu_header_e == 0)
                    {
                        // Start of Fragment.
                        // Initiise the fragmented_nal byte array
                        // Build the NAL header with the original F and NRI flags but use the the Type field from the fu_header_type
                        byte reconstructed_nal_type = (byte)((nal_header_f_bit << 7) + (nal_header_nri << 5) + fu_header_type);

                        // Empty the stream
                        fragmented_nal.SetLength(0);

                        // Add reconstructed_nal_type byte to the memory stream
                        fragmented_nal.WriteByte(reconstructed_nal_type);

                        // copy the rest of the RTP payload to the memory stream
                        fragmented_nal.Write(rtp_payloads[payload_index], 2, rtp_payloads[payload_index].Length - 2);
                    }

                    if (fu_header_s == 0 && fu_header_e == 0)
                    {
                        // Middle part of Fragment
                        // Append this payload to the fragmented_nal
                        // Data starts after the NAL Unit Type byte and the FU Header byte
                        fragmented_nal.Write(rtp_payloads[payload_index], 2, rtp_payloads[payload_index].Length - 2);
                    }

                    if (fu_header_s == 0 && fu_header_e == 1)
                    {
                        // End part of Fragment
                        // Append this payload to the fragmented_nal
                        // Data starts after the NAL Unit Type byte and the FU Header byte
                        fragmented_nal.Write(rtp_payloads[payload_index], 2, rtp_payloads[payload_index].Length - 2);

                        // Add the NAL to the array of NAL units
                        nal_units.Add(fragmented_nal.ToArray());
                    }
                }

                else if (nal_header_type == 29)
                {
                    _logger.Debug("Frag FU-B not supported");
                    fu_b++;
                }
                else
                {
                    _logger.Debug("Unknown NAL header " + nal_header_type + " not supported");
                }

            }

            // Output some statistics
            _logger.Debug("Norm=" + norm + " ST-A=" + stap_a + " ST-B=" + stap_b + " M16=" + mtap16 + " M24=" + mtap24 + " FU-A=" + fu_a + " FU-B=" + fu_b);

            // Output all the NALs that form one RTP Frame (one frame of video)
            return nal_units;

        }


    }
}
