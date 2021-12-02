using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using dynamixel_sdk;
using System.Threading;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;


namespace motor_control_with_window
{
    public partial class Form1 : Form
    {
        [Serializable]
        struct Skeleton
        {
            public double No1x;
            public double No2x;
            public double No3x;
            public double No4x;
            public double No5x;
            public double No6x;
            public double No7x;
            public double No8x;
            public double No9x;
            public double No10x;
            public double No11x;
            public double No1y;
            public double No2y;
            public double No3y;
            public double No4y;
            public double No5y;
            public double No6y;
            public double No7y;
            public double No8y;
            public double No9y;
            public double No10y;
            public double No11y;
            public double No1z;
            public double No2z;
            public double No3z;
            public double No4z;
            public double No5z;
            public double No6z;
            public double No7z;
            public double No8z;
            public double No9z;
            public double No10z;
            public double No11z;

        }

        Thread th;
        Thread test;
        
        static TcpListener listener;

    // Control table address
    public const int ADDR_PRO_TORQUE_ENABLE = 64;                 // Control table address is different in Dynamixel model
        public const int ADDR_PRO_GOAL_POSITION = 116;
        public const int ADDR_PRO_PRESENT_POSITION = 132;

        // Data Byte Length
        public const int LEN_PRO_GOAL_POSITION = 4;
        public const int LEN_PRO_PRESENT_POSITION = 4;

        // Protocol version
        public const int PROTOCOL_VERSION = 2;                   // See which protocol version is used in the Dynamixel

        // Default setting
        public const int DXL_ID = 1;                          // Dynamixel ID: 1
        public const int BAUDRATE = 1000000;
        public const string DEVICENAME = "COM4";              // Check which port is being used on your controller
                                                              // ex) Windows: "COM1"   Linux: "/dev/ttyUSB0" Mac: "/dev/tty.usbserial-*"

        public const int TORQUE_ENABLE = 1;                   // Value for enabling the torque
        public const int TORQUE_DISABLE = 0;                   // Value for disabling the torque
        public const int DXL_MINIMUM_POSITION_VALUE = 0;                   // Dynamixel will rotate between this value
        public const int DXL_MAXIMUM_POSITION_VALUE = 4095;                // and this value (note that the Dynamixel would not move when the position value is out of movable range. Check e-manual about the range of the Dynamixel you use.)
        //public const int DXL_MOVING_STATUS_THRESHOLD = 20;                  // Dynamixel moving status threshold

        //public const byte ESC_ASCII_VALUE = 0x1b;

        public const int COMM_SUCCESS = 0;                   // Communication Success result value
        public const int COMM_TX_FAIL = -1001;               // Communication Tx Failed

        int port_num;
        int groupwrite_num;
        int groupread_num;

        public int dxl_present_position1;
        public int dxl_present_position2;
        public int dxl_present_position3;
        public int dxl_present_position4;
        public int dxl_present_position5;

        public int dxl_present_position6;
        public int dxl_present_position7;
        public int dxl_present_position8;
        public int dxl_present_position9;
        public int dxl_present_position10;

        public int dxl_present_position11;
        public int dxl_present_position12;

        public double kinect_joint_1x;
        public double kinect_joint_1y;
        public double kinect_joint_1z;
        public double kinect_joint_2x;
        public double kinect_joint_2y;
        public double kinect_joint_2z;
        public double kinect_joint_3x;
        public double kinect_joint_3y;
        public double kinect_joint_3z;
        public double kinect_joint_4x;
        public double kinect_joint_4y;
        public double kinect_joint_4z;
        public double kinect_joint_5x;
        public double kinect_joint_5y;
        public double kinect_joint_5z;
        public double kinect_joint_6x;
        public double kinect_joint_6y;
        public double kinect_joint_6z;
        public double kinect_joint_7x;
        public double kinect_joint_7y;
        public double kinect_joint_7z;
        public double kinect_joint_8x;
        public double kinect_joint_8y;
        public double kinect_joint_8z;
        public double kinect_joint_9x;
        public double kinect_joint_9y;
        public double kinect_joint_9z;
        public double kinect_joint_10x;
        public double kinect_joint_10y;
        public double kinect_joint_10z;
        public double kinect_joint_11x;
        public double kinect_joint_11y;
        public double kinect_joint_11z;

        int dxl_comm_result = COMM_TX_FAIL;
        byte dxl_error = 0;                                                   // Dynamixel error
        bool dxl_addparam_result = false;                                     // AddParam result
        bool dxl_getdata_result = false;                                      // GetParam result


        public Form1()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            // Initialize PortHandler Structs
            // Set the port path
            // Get methods and members of PortHandlerLinux or PortHandlerWindows
            port_num = dynamixel.portHandler(DEVICENAME);

            // Initialize PacketHandler Structs
            dynamixel.packetHandler();

            // Initialize GroupBulkWrite Struct
            groupwrite_num = dynamixel.groupBulkWrite(port_num, PROTOCOL_VERSION);

            // Initialize Groupbulkread Structs
            groupread_num = dynamixel.groupBulkRead(port_num, PROTOCOL_VERSION);



            // Open port
            if (dynamixel.openPort(port_num))
            {
                textBoxOpenPort.Text = "Succeeded to open the port!";
            }
            else
            {
                textBoxOpenPort.Text = "Failed to open the port!\n";
                textBoxOpenPort.Text = "Press any key to terminate...";
                return;
            }

            // Set port baudrate
            if (dynamixel.setBaudRate(port_num, BAUDRATE))
            {
                textBoxBaudrate.Text = "Succeeded to change the baudrate!";
            }
            else
            {
                textBoxBaudrate.Text = "Failed to change the baudrate!";
                textBoxBaudrate.Text = "Press any key to terminate...";
                return;
            }

            // Enable Dynamixel Torque
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, DXL_ID, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC1.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC1.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC1.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 2, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC2.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC2.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC2.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 3, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC3.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC3.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC3.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 4, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC4.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC4.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC4.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 5, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC5.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC5.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC5.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 6, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC6.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC6.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC6.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 7, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC7.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC7.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC7.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 8, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC8.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC8.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC8.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 9, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC9.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC9.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC9.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 10, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC10.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC10.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC10.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 11, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC11.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC11.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC11.Text = "Dynamixel has been successfully connected";
            }

            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 12, ADDR_PRO_TORQUE_ENABLE, TORQUE_ENABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
            {
                textBoxC12.Text = Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result));
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, PROTOCOL_VERSION)) != 0)
            {
                textBoxC12.Text = Marshal.PtrToStringAnsi(dynamixel.getRxPacketError(PROTOCOL_VERSION, dxl_error));
            }
            else
            {
                textBoxC12.Text = "Dynamixel has been successfully connected";
            }


            //Read AddParamatar Part

            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 1, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 1 ] groupBulkRead addparam failed","GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 2, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 2 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 3, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 3 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 4, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 4 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 5, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 5 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }


            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 6, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 6 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 7, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 7 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 8, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 8 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 9, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 9 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 10, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 10 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }


            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 11, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 11 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }
            dxl_addparam_result = dynamixel.groupBulkReadAddParam(groupread_num, 12, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_addparam_result != true)
            {
                MessageBox.Show("[ID: 12 ] groupBulkRead addparam failed", "GroupBulkReadError");
                Close();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //trackBar Part
        
            trackBar1.Enabled = true;
            trackBar2.Enabled = true;
            trackBar3.Enabled = true;
            trackBar4.Enabled = true;
            trackBar6.Enabled = true;

            trackBar5.Enabled = true;
            trackBar7.Enabled = true;
            trackBar8.Enabled = true;
            trackBar9.Enabled = true;
            trackBar10.Enabled = true;

            trackBar11.Enabled = true;
            trackBar12.Enabled = true;

            trackBar1.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar1.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar2.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar2.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar3.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar3.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar4.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar4.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar5.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar5.Minimum = DXL_MINIMUM_POSITION_VALUE;

            trackBar6.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar6.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar7.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar7.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar8.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar8.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar9.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar9.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar10.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar10.Minimum = DXL_MINIMUM_POSITION_VALUE;

            trackBar11.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar11.Minimum = DXL_MINIMUM_POSITION_VALUE;
            trackBar12.Maximum = DXL_MAXIMUM_POSITION_VALUE;
            trackBar12.Minimum = DXL_MINIMUM_POSITION_VALUE;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //present_position & trackBar.Value Part 버튼누르고 현재 위치로 트랙바 값 초기화 파트

            dynamixel.groupBulkReadTxRxPacket(groupread_num);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
                MessageBox.Show(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)),"Error");


            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 1, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 1] groupBulkRead getdata failed","Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 2, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 2] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 3, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 3] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 4, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 4] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 5, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 5] groupBulkRead getdata failed", "Error");
                Close();
            }


            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 6, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 6] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 7, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 7] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 8, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 8] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 9, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 9] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 10, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 10] groupBulkRead getdata failed", "Error");
                Close();
            }


            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 11, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 11] groupBulkRead getdata failed", "Error");
                Close();
            }
            dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 12, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            if (dxl_getdata_result != true)
            {
                MessageBox.Show("[ID: 12] groupBulkRead getdata failed", "Error");
                Close();
            }

            dxl_present_position1 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 1, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position2 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 2, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position3 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 3, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position4 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 4, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position5 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 5, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);

            dxl_present_position6 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 6, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position7 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 7, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position8 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 8, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position9 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 9, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position10 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 10, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);

            dxl_present_position11 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 11, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
            dxl_present_position12 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 12, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);

            trackBar1.Value = dxl_present_position1;
            trackBar2.Value = dxl_present_position2;
            trackBar3.Value = dxl_present_position3;
            trackBar4.Value = dxl_present_position4;
            trackBar5.Value = dxl_present_position5;

            trackBar6.Value = dxl_present_position6;
            trackBar7.Value = dxl_present_position7;
            trackBar8.Value = dxl_present_position8;
            trackBar9.Value = dxl_present_position9;
            trackBar10.Value = dxl_present_position10;

            trackBar11.Value = dxl_present_position11;
            trackBar12.Value = dxl_present_position12;

            
            textBoxGP1.Text = trackBar1.Value.ToString();
            textBoxGP2.Text = trackBar2.Value.ToString();
            textBoxGP3.Text = trackBar3.Value.ToString();
            textBoxGP4.Text = trackBar4.Value.ToString();
            textBoxGP5.Text = trackBar6.Value.ToString();

            textBoxGP6.Text = trackBar5.Value.ToString();
            textBoxGP7.Text = trackBar7.Value.ToString();
            textBoxGP8.Text = trackBar8.Value.ToString();
            textBoxGP9.Text = trackBar9.Value.ToString();
            textBoxGP10.Text = trackBar10.Value.ToString();

            textBoxGP11.Text = trackBar11.Value.ToString();
            textBoxGP12.Text = trackBar12.Value.ToString();

            test = new Thread(Thread_Test1);
            test.Start();

        }

        public void Thread_Test1()
        {
            while (true)
            {
                dynamixel.groupBulkReadTxRxPacket(groupread_num);
                if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, PROTOCOL_VERSION)) != COMM_SUCCESS)
                    MessageBox.Show(Marshal.PtrToStringAnsi(dynamixel.getTxRxResult(PROTOCOL_VERSION, dxl_comm_result)), "Error");


                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 1, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 1] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 2, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 2] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 3, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 3] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 4, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 4] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 5, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 5] groupBulkRead getdata failed", "Error");
                    Close();
                }


                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 6, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 6] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 7, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 7] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 8, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 8] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 9, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 9] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 10, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 10] groupBulkRead getdata failed", "Error");
                    Close();
                }


                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 11, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 11] groupBulkRead getdata failed", "Error");
                    Close();
                }
                dxl_getdata_result = dynamixel.groupBulkReadIsAvailable(groupread_num, 12, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                if (dxl_getdata_result != true)
                {
                    MessageBox.Show("[ID: 12] groupBulkRead getdata failed", "Error");
                    Close();
                }

                dxl_present_position1 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 1, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP1, dxl_present_position1.ToString());
                dxl_present_position2 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 2, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP2, dxl_present_position2.ToString());
                dxl_present_position3 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 3, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP3, dxl_present_position3.ToString());
                dxl_present_position4 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 4, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP4, dxl_present_position4.ToString());
                dxl_present_position5 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 5, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP5, dxl_present_position5.ToString());

                dxl_present_position6 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 6, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP6, dxl_present_position6.ToString());
                dxl_present_position7 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 7, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP7, dxl_present_position7.ToString());
                dxl_present_position8 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 8, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP8, dxl_present_position8.ToString());
                dxl_present_position9 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 9, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP9, dxl_present_position9.ToString());
                dxl_present_position10 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 10, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP10, dxl_present_position10.ToString());

                dxl_present_position11 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 11, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP11, dxl_present_position11.ToString());
                dxl_present_position12 = (Int32)dynamixel.groupBulkReadGetData(groupread_num, 12, ADDR_PRO_PRESENT_POSITION, LEN_PRO_PRESENT_POSITION);
                CSendText(textBoxPP12, dxl_present_position12.ToString());

                // 0.1초 단위
                Thread.Sleep(10);
            }
        }

        delegate void SendText(Control ctl, String text);

        private void CSendText(Control ctl, String text)
        {
            if (ctl.InvokeRequired)
                ctl.Invoke(new SendText(CSendText), ctl, text);
            else
                ctl.Text = text;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBoxGP1.Text = trackBar1.Value.ToString();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBoxGP2.Text = trackBar2.Value.ToString();

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBoxGP3.Text = trackBar3.Value.ToString();

        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            textBoxGP4.Text = trackBar4.Value.ToString();


        }
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            textBoxGP5.Text = trackBar5.Value.ToString();

        
        }
        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            textBoxGP6.Text = trackBar6.Value.ToString();


        }
        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            textBoxGP7.Text = trackBar7.Value.ToString();

 
        }
        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            textBoxGP8.Text = trackBar8.Value.ToString();

     
        }
        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            textBoxGP9.Text = trackBar9.Value.ToString();

        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            textBoxGP10.Text = trackBar10.Value.ToString();

        }
        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            textBoxGP11.Text = trackBar11.Value.ToString();


        }
        private void trackBar12_Scroll(object sender, EventArgs e)
        {
            textBoxGP12.Text = trackBar12.Value.ToString();


        }

        public static double ConvertToDouble(string Value)
        {
            if (Value == null)
            {
                return 0;
            }
            else
            {
                double OutVal;
                double.TryParse(Value, out OutVal);

                if (double.IsNaN(OutVal) || double.IsInfinity(OutVal))
                {
                    return 0;
                }
                return OutVal;
            }
        }

        public void Deserialize(byte[] data)
        {


            Skeleton std = new Skeleton();

            int size_skeleton = Marshal.SizeOf(std);
            byte[] skeletonData = new byte[size_skeleton];
            Array.Copy(data, 1, skeletonData, 0, size_skeleton);
            GCHandle handle = GCHandle.Alloc(skeletonData, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            std = (Skeleton)Marshal.PtrToStructure(buffer, typeof(Skeleton));

            kinect_joint_1x = std.No1x;
            kinect_joint_1y = std.No1y;
            kinect_joint_1z = std.No1z;
            kinect_joint_2x = std.No2x;
            kinect_joint_2y = std.No2y;
            kinect_joint_2z = std.No2z;
            kinect_joint_3x = std.No3x;
            kinect_joint_3y = std.No3y;
            kinect_joint_3z = std.No3z;
            kinect_joint_4x = std.No4x;
            kinect_joint_4y = std.No4y;
            kinect_joint_4z = std.No4z;
            kinect_joint_5x = std.No5x;
            kinect_joint_5y = std.No5y;
            kinect_joint_5z = std.No5z;
            kinect_joint_6x = std.No6x;
            kinect_joint_6y = std.No6y;
            kinect_joint_6z = std.No6z;
            kinect_joint_7x = std.No7x;
            kinect_joint_7y = std.No7y;
            kinect_joint_7z = std.No7z;
            kinect_joint_8x = std.No8x;
            kinect_joint_8y = std.No8y;
            kinect_joint_8z = std.No8z;
            kinect_joint_9x = std.No9x;
            kinect_joint_9y = std.No9y;
            kinect_joint_9z = std.No9z;
            kinect_joint_10x = std.No10x;
            kinect_joint_10y = std.No10y;
            kinect_joint_10z = std.No10z;
            kinect_joint_11x = std.No11x;
            kinect_joint_11y = std.No11y;
            kinect_joint_11z = std.No11z;

            textBox3.Text = kinect_joint_1x.ToString();
            textBox1.Text = kinect_joint_1y.ToString();
            textBox2.Text = kinect_joint_1z.ToString();
            textBox6.Text = kinect_joint_2x.ToString();
            textBox5.Text = kinect_joint_2x.ToString();
            textBox4.Text = kinect_joint_2x.ToString();
            textBox9.Text = kinect_joint_3x.ToString();
            textBox8.Text = kinect_joint_3y.ToString();
            textBox7.Text = kinect_joint_3z.ToString();
            textBox12.Text = kinect_joint_4x.ToString();
            textBox11.Text = kinect_joint_4y.ToString();
            textBox10.Text = kinect_joint_4z.ToString();
            textBox15.Text = kinect_joint_5x.ToString();
            textBox14.Text = kinect_joint_5y.ToString();
            textBox13.Text = kinect_joint_5z.ToString();
            textBox18.Text = kinect_joint_6x.ToString();
            textBox17.Text = kinect_joint_6y.ToString();
            textBox16.Text = kinect_joint_6z.ToString();
            textBox21.Text = kinect_joint_7x.ToString();
            textBox20.Text = kinect_joint_7y.ToString();
            textBox19.Text = kinect_joint_7z.ToString();
            textBox24.Text = kinect_joint_8x.ToString();
            textBox23.Text = kinect_joint_8y.ToString();
            textBox22.Text = kinect_joint_8z.ToString();
            textBox27.Text = kinect_joint_9x.ToString();
            textBox26.Text = kinect_joint_9y.ToString();
            textBox25.Text = kinect_joint_9z.ToString();
            textBox30.Text = kinect_joint_10x.ToString();
            textBox29.Text = kinect_joint_10y.ToString();
            textBox28.Text = kinect_joint_10z.ToString();
            textBox33.Text = kinect_joint_11x.ToString();
            textBox32.Text = kinect_joint_11y.ToString();
            textBox31.Text = kinect_joint_11z.ToString();
        }

        private void TrackBar1_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void StartServer(int portNum)
        {
    
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), portNum);
            listener.Start();
          
            
        }

        private void buttonOpenServer_Click(object sender, EventArgs e)
        {
            StartServer(1);
            MessageBox.Show("서버열림!");

        }

        public void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();
                soc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                
                try
                {
                    Stream ss = new NetworkStream(soc);
                    StreamWriter ssw = new StreamWriter(ss);
                    StreamReader ssr = new StreamReader(ss);
                    BinaryReader bssr = new BinaryReader(ss);
                    BinaryWriter bssw = new BinaryWriter(ss);

                    while (true)
                    {
                       
                        byte[] buffer = new byte[512];

                        int iReadLength = bssr.Read(buffer, 0, 512);
                        if (iReadLength > 0)
                        {
                                Deserialize(buffer);
                        }
                        
                    }
                    
                }
                catch (Exception e)
                {
#if LOG
                        Console.WriteLine(e.Message);
#endif
                }
#if LOG
                        Console.WriteLine("Disconnected: {0}", soc.RemoteEndPoint);
#endif
                soc.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            th = new Thread(Service);
            th.Start();
            
            MessageBox.Show("쓰레드 열림!");
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            th.Abort();
            test.Abort();
            listener.Stop();
            MessageBox.Show("쓰레드와 서버 닫힘!");
        }

        private void buttonTorqueDisable_Click(object sender, EventArgs e)
        {
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 1, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 2, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 3, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 4, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 5, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 6, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 7, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 8, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 9, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 10, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 11, ADDR_PRO_TORQUE_ENABLE, 0);
            dynamixel.write1ByteTxRx(port_num, PROTOCOL_VERSION, 12, ADDR_PRO_TORQUE_ENABLE, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName;
            string filePath;
            int numOfFile = 0;

            string currentPath = Directory.GetCurrentDirectory();
            currentPath += "\\SaveMt";

            if (Directory.Exists(currentPath))
            {
                Console.WriteLine("경로 존재");
            }
            else
            {
                Directory.CreateDirectory(currentPath);

            }

      
            int len = numOfFile + 1;

            fileName = "sample.txt";


            filePath = currentPath + "\\" + fileName;

            FileStream fileStream = new FileStream(
                    filePath,          
                    FileMode.Append,      
                    FileAccess.Write      
                    );

            StreamWriter streamWriter = new StreamWriter(
                fileStream,           
                Encoding.UTF8          
            );

            streamWriter.Write(kinect_joint_1x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_1y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_1z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_2x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_2y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_2z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_3x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_3y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_3z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_4x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_4y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_4z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_5x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_5y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_5z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_6x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_6y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_6z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_7x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_7y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_7z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_8x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_8y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_8z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_9x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_9y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_9z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_10x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_10y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_10z);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_11x);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_11y);
            streamWriter.Write("\t");
            streamWriter.Write(kinect_joint_11z);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position1);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position2);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position3);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position4);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position5);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position6);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position7);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position8);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position9);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position10);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position11);
            streamWriter.Write("\t");
            streamWriter.Write(dxl_present_position12);
            streamWriter.Write("\n");

            //스트림 Writer 닫아 주세요.
            streamWriter.Close();

            //파일스트림을 다 썼다면 반드시 닫아 주세요.
            fileStream.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
