using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SRTPluginProviderRE3
{
    /// <summary>
    /// SHA256 hashes for the RE3/BIO3 REmake game executables.
    /// 
    /// Resident Evil 3 (WW): https://steamdb.info/app/952060/ / https://steamdb.info/depot/952062/
    /// Biohazard 3 (CERO Z): https://steamdb.info/app/1100830/ / https://steamdb.info/depot/1100831/
    /// </summary>
    public static class GameHashes
    {
        // Latest RT DX12 Build
        private static readonly byte[] re3WW_11026988 = new byte[32] { 0x53, 0x01, 0x96, 0x79, 0x57, 0xf6, 0x8b, 0x61, 0xc1, 0xe2, 0x2b, 0xe5, 0x74, 0xe0, 0x87, 0xd5, 0x6f, 0x49, 0xa5, 0x2f, 0xb5, 0xde, 0xad, 0x5f, 0x4c, 0xf0, 0x17, 0xb9, 0xf4, 0x03, 0xb9, 0xac };
        // Latest DX11 Build
        private static readonly byte[] re3WW_11047294 = new byte[32] { 0xf7, 0xf7, 0x07, 0x74, 0x32, 0x43, 0xc0, 0x58, 0xe5, 0x65, 0x7f, 0x27, 0x0b, 0x7f, 0x9f, 0x5d, 0x01, 0x17, 0x4b, 0x0e, 0x2e, 0x5b, 0x67, 0x94, 0x61, 0xc2, 0x39, 0x2a, 0xe8, 0x13, 0x0a, 0x30 };
        // Latest CeroD RT DX12 Build TO-DO
        private static readonly byte[] re3cerod_11026646 = new byte[32] { 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        // Latest CeroD RT DX11 Build TO-DO
        private static readonly byte[] re3cerod_11047603 = new byte[32] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

        private static void OutputVersionString(byte[] cs)
        {
            StringBuilder sb = new StringBuilder("private static readonly byte[] re3??_00000000 = new byte[32] { ");

            for (int i = 0; i < cs.Length; i++)
            {
                sb.AppendFormat("0x{0:X2}", cs[i]);

                if (i < cs.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(" }");
            Console.WriteLine("Please DM VideoGameRoulette or Squirrelies with the version.log");
            // write output to file
            string filename = "version.log";
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(sb.ToString());
            }
        }

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(re3WW_11026988))
            {
                Console.WriteLine("Game Detected! Version: World public Release");
                return GameVersion.RE3_WW_11026988;
            }
                
            else if (checksum.SequenceEqual(re3WW_11047294))
            {
                Console.WriteLine("Game Detected! Version: World dx11_non-rt Release");
                return GameVersion.RE3_WW_11047294;
            }
            else if (checksum.SequenceEqual(re3cerod_11026646))
            {
                Console.WriteLine("Game Detected! Version: CeroD public Release");
                return GameVersion.RE3_CEROD_11026646;
            }
            else if (checksum.SequenceEqual(re3cerod_11047603))
            {
                Console.WriteLine("Game Detected! Version: CeroD dx11_non-rt Release");
                return GameVersion.RE3_CEROD_11047603;
            }
            else
            {
                Console.WriteLine("Unknown Version");
                OutputVersionString(checksum);
                return GameVersion.Unknown;
            }
        }
    }
}
