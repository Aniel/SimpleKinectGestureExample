using Microsoft.Kinect;
using SimpleKinectGestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleKinectGestureExample
{
	class Program
	{
		private static CoordinateMapper _coordinateMapper;
		private static HandOverHeadDetector _handOverHeadDetector;
		private static Body[] _bodies;
		private static BodyFrameReader _bodyFrameReader;
		private static KinectSensor _kinectSensor;

		static void Main(string[] args)
		{
			RunAsync().Wait();
			Console.ReadKey();
		}

		//Run the application async
		static async Task RunAsync()
		{
			//Get the default Kinect Sensor
			_kinectSensor = KinectSensor.GetDefault();

			// open the reader for the body frames
			_bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();

			// Set the coordinate Mapper
			_coordinateMapper = _kinectSensor.CoordinateMapper;

			//open the sensor
			_kinectSensor.Open();

			//Check if the Sensor is available
			Console.WriteLine("Kinect sensor is " + (_kinectSensor.IsAvailable ? "available " : "missing. Waiting for sensor: press ctrl + c to abort"));
			while (!_kinectSensor.IsAvailable)
			{
				//wait for sensor
			}
			Console.WriteLine("Kinect sensor is " + (_kinectSensor.IsAvailable ? "available " : "missing. Waiting for sensor: press ctrl + c to abort"));

			//Init gesture
			_handOverHeadDetector = new HandOverHeadDetector(HandDetectionType.BothHands, HandState.Open);
			//Subscribe to completed event
			_handOverHeadDetector.GestureCompleteEvent += HandOverHeadDetectorOnGestureCompleteEvent;

			//Start reciving kinect Frames
			if (_bodyFrameReader != null)
			{
				_bodyFrameReader.FrameArrived += Reader_FrameArrived;
			}
		}

		//Display completed event
		private static void HandOverHeadDetectorOnGestureCompleteEvent(object sender, EventArgs e)
		{
			Console.WriteLine("Your hand is over your head!");
		}

		//Process Frames
		private static void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
		{
			var dataReceived = false;
			using (var bodyFrame = e.FrameReference.AcquireFrame())
			{
				if (bodyFrame != null)
				{
					if (_bodies == null)
					{
						_bodies = new Body[bodyFrame.BodyCount];
					}
					bodyFrame.GetAndRefreshBodyData(_bodies);
					dataReceived = true;
				}
			}

			if (dataReceived)
			{
				//Check if hand is over the head
				_handOverHeadDetector.UpdateData(_bodies);

			}
		}
	}
}
