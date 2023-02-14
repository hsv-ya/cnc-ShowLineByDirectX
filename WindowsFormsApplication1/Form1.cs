using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using Microsoft.DirectX;
using Microsoft.DirectX.PrivateImplementationDetails;
using Microsoft.DirectX.Direct3D;
using System.IO;


namespace WindowsFormsApplication1 {
	public partial class Form1 : Form {

		private Device device = null;
		private Limits2D ScreenLimits = new Limits2D();
		private Color backgroundColor = Color.LightBlue;
		private bool CameraViewSet = false;
		static private CameraViewType CameraView = CameraViewType.TopView;
		private float ZFarPlane = 1000.0f;
		private float ZNearPlane = -500.0f;
		private Vector3 CameraLocation = new Vector3(0f, 0f, 50f);
		private Vector3 CameraUpVector = new Vector3(0f, 1f, 0f);
		private bool needRendering = false;
		static private List<Vector3> allPoints = new List<Vector3>();
		static private CustomVertex.PositionColored[] array = null;
		private bool captureMouse = false;
		private int deltaX = 0;
		private int deltaY = 0;
		private int oldX = 0;
		private int oldY = 0;
		private Vector3 delta = new Vector3();
		private bool captureMouseAngle = false;
		private Vector3 angle_delta = new Vector3();
		private ObjectLimit objectLimits = new ObjectLimit();
		private int zoomX = 0;
		private int zoomY = 0;
		static private float scaleKoef = 0.05f;

		public struct Limits2D {
			public float Left, Right;
			public float Top, Bottom;
		}

		public struct ObjectLimit {
			public float Left, Right; // X-+
			public float Front, Back; // Y+-
			public float Top, Bottom; // Z+-
		}

		public enum CameraViewType {
			TopView,
			//BottomView,
			FrontView,
			//BackView,
			RigthView,
			//LeftView,
			IsoView
		}
		
		public Form1() {
			InitializeComponent();

			pScreen.MouseWheel += new MouseEventHandler(pScreen_MouseWheel);
		}

		private void btnCreateDevice_Click(object sender, EventArgs e) {
			if (device == null)
				InitializeGraphics();
			else
				labelLog.Text = "device already init!";
		}

		private void initScreenLimits() {
			ScreenLimits.Left = 0;
			ScreenLimits.Right = pScreen.Size.Width;
			ScreenLimits.Top = pScreen.Size.Height;
			ScreenLimits.Bottom = 0;

			zoomX = (int)(pScreen.Size.Width * scaleKoef);
			zoomY = (int)(pScreen.Size.Height * scaleKoef);
		}

		public bool InitializeGraphics() {
			PresentParameters prePars = new PresentParameters();
			prePars.Windowed = true;
			prePars.SwapEffect = SwapEffect.Discard;

			initScreenLimits();

			try {
				device = new Device(0, DeviceType.Hardware, pScreen, CreateFlags.SoftwareVertexProcessing, prePars);
				labelLog.Text = "DirectX initialized";

				return true;
			} catch (DirectXException e) {
				labelLog.Text = "Error initialization DirectX " + e.ErrorString;
				MessageBox.Show(labelLog.Text);
				return false;
			}
		}

		private void SetupCamera() {
			if (CameraViewSet) {
				CameraViewSet = false;
				if (CameraView == CameraViewType.TopView) {
					device.Transform.World = Matrix.RotationYawPitchRoll(0f, 0f, 0f);
				} else if (CameraView == CameraViewType.FrontView) {
					device.Transform.World = Matrix.RotationYawPitchRoll(0f, -1.57f, 0f);
				} else if (CameraView == CameraViewType.RigthView) {
					device.Transform.World = Matrix.RotationYawPitchRoll(0f, -1.57f, -1.57f);
				} else if (CameraView == CameraViewType.IsoView) {
					device.Transform.World = Matrix.RotationYawPitchRoll(0f, -0.785f, 0.785f);
				}
			}
			// NOTE: Parallel
			device.Transform.Projection = Matrix.OrthoOffCenterRH
			// NOTE: Perspective
			//device.Transform.Projection = Matrix.PerspectiveOffCenterRH
			(ScreenLimits.Left, ScreenLimits.Right, ScreenLimits.Bottom, ScreenLimits.Top, ZNearPlane, ZFarPlane);
			device.Transform.View = Matrix.LookAtRH(new Vector3(CameraLocation.X, CameraLocation.Y, CameraLocation.Z), new Vector3(CameraLocation.X + angle_delta.X, CameraLocation.Y + angle_delta.Y, 0f), CameraUpVector);
			device.RenderState.Lighting = false;
		}

		private void rendering() {
			if (device == null)
				return;

			device.Clear(ClearFlags.Target, backgroundColor, 1.0f, 0);

			SetupCamera();

			device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Position;

			if (allPoints.Count > 0) {
				device.BeginScene();
				device.DrawUserPrimitives(PrimitiveType.LineStrip, array.Length - 1, array);
			} else
				device.BeginScene();

			device.EndScene();
			device.Present();
		}

		private void pScreen_Paint(object sender, PaintEventArgs e) {
			rendering();
		}

		private void button2_Click(object sender, EventArgs e) {
			if (needRendering)
				return;
			needRendering = true;
			button2.Enabled = false;
			while (needRendering) {
				rendering();
				Application.DoEvents();
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			needRendering = false;
		}

		private void btnLoadCNC_Click(object sender, EventArgs e) {
			bool needInitLimits = true;
			int iLine = 0;
			string s = "";
			Vector3 point;
			TextReader textFile = File.OpenText("Temp22.cnc");
			allPoints.Clear();
			while ((s = textFile.ReadLine()) != null) {
				iLine++;
				point = new Vector3();
				try {
					point.X = (float)Convert.ToDouble(s);
					if (needInitLimits) {
						objectLimits.Left = point.X;
						objectLimits.Right = point.X;
					}
					objectLimits.Right = Math.Max(point.X, objectLimits.Right);
					objectLimits.Left = Math.Min(point.X, objectLimits.Left);
				} catch {
					labelLog.Text = "Error in line: " + iLine.ToString();
					break;
				}
				s = textFile.ReadLine();
				iLine++;
				try {
					point.Y = (float)Convert.ToDouble(s) * -1.0f;
					if (needInitLimits) {
						objectLimits.Front = point.Y;
						objectLimits.Back = point.Y;
					}
					objectLimits.Front = Math.Max(point.Y, objectLimits.Front);
					objectLimits.Back = Math.Min(point.Y, objectLimits.Back);
				} catch {
					labelLog.Text = "Error in line: " + iLine.ToString();
					break;
				}
				s = textFile.ReadLine();
				iLine++;
				try {
					point.Z = (float)Convert.ToDouble(s);
					if (needInitLimits) {
						objectLimits.Top = point.Z;
						objectLimits.Bottom = point.Z;
					}
					objectLimits.Top = Math.Max(point.Z, objectLimits.Top);
					objectLimits.Bottom = Math.Min(point.Z, objectLimits.Bottom);
				} catch {
					labelLog.Text = "Error in line: " + iLine.ToString();
					break;
				}
				allPoints.Add(point);
				needInitLimits = false;
			}
			textFile.Close();
			s = "Loaded " + allPoints.Count.ToString() + " points.";
			s += "Limits: X[" + objectLimits.Left.ToString() + ", " + objectLimits.Right.ToString() + "], ";
			s += "Limits: Y[" + objectLimits.Back.ToString() + ", " + objectLimits.Front.ToString() + "], ";
			s += "Limits: Z[" + objectLimits.Bottom.ToString() + ", " + objectLimits.Top.ToString() + "]";
			labelLog.Text = s;
			array = new CustomVertex.PositionColored[allPoints.Count];

			for (int i = 0; i < allPoints.Count; i++) {
				array[i].Color = Color.Black.ToArgb();
				array[i].Position = allPoints[i];
			}
		}

		private void renderNewCameraViewType(CameraViewType newView) {
			initScreenLimits();
			angle_delta.X = 0;
			angle_delta.Y = 0;
			CameraLocation.X = 0;
			CameraLocation.Y = 0;
			CameraView = newView;
			if (newView == CameraViewType.TopView) {
				CameraLocation.Y = -(pScreen.Size.Height - Math.Abs(objectLimits.Top - objectLimits.Bottom));
			} else if (newView == CameraViewType.RigthView)
				CameraLocation.X = -(pScreen.Size.Width - Math.Abs(objectLimits.Left - objectLimits.Right) / 2);
			CameraViewSet = true;
			rendering();
		}

		private void btnTop_Click(object sender, EventArgs e) {
			renderNewCameraViewType(CameraViewType.TopView);
		}

		private void button5_Click(object sender, EventArgs e) {
			renderNewCameraViewType(CameraViewType.FrontView);
		}

		private void button6_Click(object sender, EventArgs e) {
			renderNewCameraViewType(CameraViewType.RigthView);
		}

		private void button7_Click(object sender, EventArgs e) {
			renderNewCameraViewType(CameraViewType.IsoView);
		}

		private void pScreen_MouseMove(object sender, MouseEventArgs e) {
			if (captureMouse) {
				deltaX = oldX - e.X;
				deltaY = oldY - e.Y;
				oldX = e.X;
				oldY = e.Y;
				labelLog.Text = "Mouse position: " + e.X.ToString() + ", " + e.Y.ToString() + ", delta: " + deltaX.ToString() + ", " + deltaY.ToString() +
					", Camera(" + CameraLocation.X.ToString() + ", " + CameraLocation.Y.ToString() + ")";
				
				CameraLocation.X += deltaX;
				CameraLocation.Y -= deltaY;

				rendering();
			}
			if (captureMouseAngle) {
				deltaX = oldX - e.X;
				deltaY = oldY - e.Y;
				oldX = e.X;
				oldY = e.Y;
				labelLog.Text = "Mouse position: " + e.X.ToString() + ", " + e.Y.ToString() + ", delta: " + deltaX.ToString() + ", " + deltaY.ToString() +
					", Camera(" + CameraLocation.X.ToString() + ", " + CameraLocation.Y.ToString() + ")";

				angle_delta.X -= deltaX;
				angle_delta.Y += deltaY;

				rendering();
			}
		}

		private void pScreen_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				oldX = e.X;
				oldY = e.Y;
				captureMouse = true;
				captureMouseAngle = false;
			} else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
				oldX = e.X;
				oldY = e.Y;
				captureMouse = false;
				captureMouseAngle = true;
			}
		}

		private void pScreen_MouseUp(object sender, MouseEventArgs e) {
			if ((e.Button == System.Windows.Forms.MouseButtons.Left) || (e.Button == System.Windows.Forms.MouseButtons.Right)) {
				captureMouse = false;
				captureMouseAngle = false;
			}
		}

		void pScreen_MouseWheel(object sender, MouseEventArgs e) {
			if (e.Delta > 0) {
				ScreenLimits.Left -= zoomX;
				ScreenLimits.Right += zoomX;
				ScreenLimits.Top += zoomY;
				ScreenLimits.Bottom -= zoomY;
			} else {
				ScreenLimits.Left += zoomX;
				ScreenLimits.Right -= zoomX;
				ScreenLimits.Top -= zoomY;
				ScreenLimits.Bottom += zoomY;
			}

			labelLog.Text = "";
			CameraViewSet = true;
			rendering();
		}

		private void pScreen_Click(object sender, EventArgs e) {
			pScreen.Focus();
		}
	}
}
