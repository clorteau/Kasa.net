using BitConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Networking;
using Windows.UI.Xaml.Controls;

namespace Kasa
{
    // Adapted from https://github.com/softScheck/tplink-smartplug,
    // created by Lubomir Stroetmann, Copyright 2016 softScheck GmbH and
    // licensed under the Apache License, Version 2.0.
    public static class Commander
    {
        // Return bytes containing the length of a message packed as Big Indian 
        // appended by a XOR Autokey Cipher with starting key = 171 of the message
        // which is how the TP-Link devices communicate over a network
        private static byte[] Encrypt (string source)
        {
            int key = 171;

            //consider using https://stackoverflow.com/questions/51611550/converting-big-endian-to-little-endian-in-c-sharp#51611667 to avoid unnecessary references
            EndianBitConverter converter = EndianBitConverter.BigEndian;
            byte[] biglen = converter.GetBytes(source.Length);

            List<byte> bytes = new List<byte>();
            foreach (byte b in biglen) { bytes.Add(b); }
            for (int c = 0; c < source.Length; c++)
            {
                char a = (char)(source[c] ^ key);
                key = a;
                bytes.Add(Convert.ToByte(a));
            }
            return bytes.ToArray();
        }

        public static async void SendCommand(string message, string ip)
        {
            using(var streamSocket = new Windows.Networking.Sockets.StreamSocket())
            {
                try
                {
                    await streamSocket.ConnectAsync(new HostName(ip), "9999");
                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        byte[] encrypted = Encrypt(message);
                        await outputStream.WriteAsync(encrypted, 0, encrypted.Length);
                        await outputStream.FlushAsync();
                    }
                }
                catch (Exception oops)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Communication error",
                        Content = "The following error occurred while attempting to communicate with the switch:\n\n" + oops.Message,
                        CloseButtonText = "OK"
                    };
                    await dialog.ShowAsync();
                }
            }
        }

        public static class Command
        {
            public static string On = @"{""system"":{""set_relay_state"":{""state"":1}}}";
            public static string Off = @"{""system"":{""set_relay_state"":{""state"":0}}}";
        }
    }
}
