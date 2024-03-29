﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helloworld
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        /// <summary>
        /// gui tin di
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage(txbMessage.Text);
        }

        IPEndPoint IP;
        Socket client;

        /// <summary>
        /// connect to server
        /// </summary>
        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Parse("10.192.63.61"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(IP);
            }
            catch
            {

                MessageBox.Show("Can not connect to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        /// <summary>
        /// close connect
        /// </summary>
        void Close()
        {
            client.Close();
        }

        /// <summary>
        /// send message
        /// </summary>
        void Send()
        {
            if (txbMessage.Text != string.Empty)
                client.Send(Serialize(txbMessage.Text));
        }

        /// <summary>
        /// receive message
        /// </summary>
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                client.Receive(data);

                    string message = (string)Deserialize(data);

                    AddMessage(message);
                }
            }
            catch
            {

                Close();
            }
        }

        /// <summary>
        /// add message vao khung chat
        /// </summary>
        /// <param name="s"></param>
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
            txbMessage.Clear();
        }

        /// <summary>
        /// phan manh
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        /// <summary>
        /// gom manh
        /// </summary>
        /// <returns></returns>
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        /// <summary>
        /// dong ket noi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
