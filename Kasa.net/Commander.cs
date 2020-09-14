/*
    Kasa.NET - Control Kasa TP-Link smart switches
    Copyright (C) 2020  Clem Lorteau <clem@lorteau.fr>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using BitConverter;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static byte[] Encrypt(string source)
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
            using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
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
