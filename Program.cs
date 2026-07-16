using System;
using System.IO;
using System.Text;

/*
 * LegacySDK SFB Fork
 * Original SFB by pink1stools, https://github.com/pink1stools/PS3_DISC.SFB-Editor
 * Modified by miskaa (gh:miskkaaa dc:misko.bin)
 * Made for LegacySDK PS3-Launcher
*/

namespace sfb {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1) {
                PrintHelp();
                return;
            }

            Console.WriteLine("[.] Opening file {0}...", args[0]);

            if (args.Length > 1)
            {
                if (args[1] == "--info" || args[1] == "-i")
                {
                    getInformationAboutThisMotherfuckingFile(args[0]);
                    return;
                }
            }
            if (args[0] == "--create" || args[0] == "-c")
            {
                if (args.Length < 4)
                {
                    PrintHelp();
                    return;
                }

                CreateSfb(args[1], args[2], args[3]);
                return;
            }

            Loadsfb(args[0]);
        }

        static void CreateSfb(string output, string titleId, string title)
        {
            SFB sfb = new SFB();

            // Magic
            sfb._SFB = Encoding.ASCII.GetBytes(".SFB");

            // Version
            sfb.Version = new byte[]
                {
                    0x00,0x00,0x01,0x00
                };

            // HYBRID_FLAG
            Array.Copy(
                Encoding.ASCII.GetBytes("HYBRID_FLAG"),
                sfb.H_F,
                "HYBRID_FLAG".Length);

            sfb.dcdo = BitConverter.GetBytes(0x00000200);
            Array.Reverse(sfb.dcdo);

            sfb.dcdl = BitConverter.GetBytes(0x20);
            Array.Reverse(sfb.dcdl);

            sfb.dtdo = BitConverter.GetBytes(0x00000220);
            Array.Reverse(sfb.dtdo);

            sfb.dtdl = BitConverter.GetBytes(0x10);
            Array.Reverse(sfb.dtdl);

            // TITLE_ID
            byte[] tid = Encoding.ASCII.GetBytes(titleId);
            Array.Copy(tid, sfb.Tid, Math.Min(tid.Length, 16));

            // FLAGS
            byte[] flags = Encoding.ASCII.GetBytes("PS3");
            Array.Copy(flags, sfb.Flags, flags.Length);

            // TITLE
            byte[] t = Encoding.ASCII.GetBytes(title);
            Array.Copy(t, sfb.Title, Math.Min(t.Length, 16));

            using (FileStream fs = new FileStream(output, FileMode.Create))
            {
                sfb.Write(fs);
            }

            Console.WriteLine("Created " + output);
        }

        static void PrintHelp() {
            Console.WriteLine("sfb.exe (PS3_DISC.SFB)");
            Console.WriteLine("Arguments:\n");
            Console.WriteLine("--info / -i | Shows information about the SFB file");
            Console.WriteLine("--create <output> <TITLE_ID> <TITLE> | Creates a SFB File");
        }

        static void getInformationAboutThisMotherfuckingFile(string file) {
            SFB sfb = new SFB();
            using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
            {
                sfb.Load(sourceStream);
            }

            Console.WriteLine("[*] .SFB Magic: {0}", Encoding.ASCII.GetString(sfb._SFB).Trim('\0'));
            Console.WriteLine("[*] Version: {0}", BitConverter.ToString(sfb.Version).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] HYBRID_FLAG: {0}", Encoding.ASCII.GetString(sfb.H_F).Trim('\0'));
            Console.WriteLine("[*] Disc Content Data Offset: {0}", BitConverter.ToString(sfb.dcdo).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Content Data Length: {0}", BitConverter.ToString(sfb.dcdl).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] TITLE_ID: {0}", Encoding.ASCII.GetString(sfb.Tid).Trim('\0'));
            Console.WriteLine("[*] Disc Title Data Offset: {0}", BitConverter.ToString(sfb.dtdo).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Title Data Length: {0}", BitConverter.ToString(sfb.dtdl).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Content (Flags): {0}", Encoding.ASCII.GetString(sfb.Flags).Trim('\0'));
            Console.WriteLine("[*] Disc Title: {0}", Encoding.ASCII.GetString(sfb.Title).Trim('\0'));
        }

        static void Loadsfb(string file) {
            SFB sfb = new SFB();

            using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite)) {
                sfb.Load(sourceStream);
            }

            Console.WriteLine("[*] .SFB Magic: {0}", Encoding.ASCII.GetString(sfb._SFB).Trim('\0'));
            Console.WriteLine("[*] Version: {0}", BitConverter.ToString(sfb.Version).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] HYBRID_FLAG: {0}", Encoding.ASCII.GetString(sfb.H_F).Trim('\0'));
            Console.WriteLine("[*] Disc Content Data Offset: {0}", BitConverter.ToString(sfb.dcdo).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Content Data Length: {0}", BitConverter.ToString(sfb.dcdl).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] TITLE_ID: {0}", Encoding.ASCII.GetString(sfb.Tid).Trim('\0'));
            Console.WriteLine("[*] Disc Title Data Offset: {0}", BitConverter.ToString(sfb.dtdo).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Title Data Length: {0}", BitConverter.ToString(sfb.dtdl).Replace("-", "").TrimStart('0'));
            Console.WriteLine("[*] Disc Content (Flags): {0}", Encoding.ASCII.GetString(sfb.Flags).Trim('\0'));
            Console.WriteLine("[*] Disc Title: {0}", Encoding.ASCII.GetString(sfb.Title).Trim('\0'));

            bool modified = false;

            Console.Write("\nEdit Flags (Y/N)? ");
            string input = Console.ReadLine();
            if (string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[*] Current Flags: {0}", Encoding.ASCII.GetString(sfb.Flags).Trim('\0'));
                Console.Write("New Flags (max 32 chars): ");
                string sflags = Console.ReadLine() ?? "";

                byte[] nflags2 = Encoding.ASCII.GetBytes(sflags);
                if (nflags2.Length <= 0x20)
                {
                    byte[] nflags = new byte[0x20];
                    Array.Copy(nflags2, nflags, nflags2.Length);
                    sfb.Flags = nflags;
                    modified = true;
                    Console.WriteLine("[+] Flags updated successfully.");
                }
                else
                {
                    Console.WriteLine("[-] Error: New Flags are too long!");
                }
            }

            Console.Write("\nEdit TITLE_ID (Y/N)? ");
            input = Console.ReadLine();
            if (string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[*] Current TITLE_ID: {0}", Encoding.ASCII.GetString(sfb.Tid).Trim('\0'));
                Console.Write("New TITLE_ID (max 16 chars): ");
                string sTitle = Console.ReadLine() ?? "";

                byte[] nTitle2 = Encoding.ASCII.GetBytes(sTitle);
                if (nTitle2.Length <= 0x10)
                {
                    byte[] nTitle = new byte[0x10];
                    Array.Copy(nTitle2, nTitle, nTitle2.Length);
                    sfb.Tid = nTitle;
                    modified = true;
                    Console.WriteLine("[+] TITLE_ID updated successfully.");
                }
                else
                {
                    Console.WriteLine("[-] Error: New TITLE_ID is too long!");
                }
            }

            if (modified)
            {
                Console.Write("\nSave changes (Y/N)? ");
                input = Console.ReadLine();
                if (string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.Write))
                    {
                        sfb.Write(sourceStream);
                    }
                    Console.WriteLine("[+] File saved successfully!");
                }
            }
        }
    }

    public class SFB
    {
        public byte[] _SFB = new byte[4];
        public byte[] Version = new byte[4];
        public byte[] Un1 = new byte[0x18];
        public byte[] H_F = new byte[0x10];
        public byte[] dcdo = new byte[4];
        public byte[] dcdl = new byte[4];
        public byte[] Un2 = new byte[8];
        public byte[] Tid = new byte[0x10];
        public byte[] dtdo = new byte[4];
        public byte[] dtdl = new byte[4];
        public byte[] Un3 = new byte[0x1A8];
        public byte[] Flags = new byte[0x20];
        public byte[] Title = new byte[0x10];
        public byte[] Un4 = new byte[0x3D0];

        public void Load(FileStream sfb)
        {
            sfb.Read(_SFB, 0, 4);
            sfb.Read(Version, 0, 4);
            sfb.Read(Un1, 0, 0x18);
            sfb.Read(H_F, 0, 0x10);
            sfb.Read(dcdo, 0, 4);
            sfb.Read(dcdl, 0, 4);
            sfb.Read(Un2, 0, 8);
            sfb.Read(Tid, 0, 0x10);
            sfb.Read(dtdo, 0, 4);
            sfb.Read(dtdl, 0, 4);
            sfb.Read(Un3, 0, 0x1A8);
            sfb.Read(Flags, 0, 0x20);
            sfb.Read(Title, 0, 0x10);
            sfb.Read(Un4, 0, 0x3D0);
        }

        public void Write(FileStream sfb)
        {
            sfb.Write(_SFB, 0, 4);
            sfb.Write(Version, 0, 4);
            sfb.Write(Un1, 0, 0x18);
            sfb.Write(H_F, 0, 0x10);
            sfb.Write(dcdo, 0, 4);
            sfb.Write(dcdl, 0, 4);
            sfb.Write(Un2, 0, 8);
            sfb.Write(Tid, 0, 0x10);
            sfb.Write(dtdo, 0, 4);
            sfb.Write(dtdl, 0, 4);
            sfb.Write(Un3, 0, 0x1A8);
            sfb.Write(Flags, 0, 0x20);
            sfb.Write(Title, 0, 0x10);
            sfb.Write(Un4, 0, 0x3D0);
        }
    }
}