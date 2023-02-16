using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.PrivateImplementationDetails;
using Direct3D = Microsoft.DirectX.Direct3D;
using System.IO;


namespace WindowsFormsApplication1 {
	public partial class Form1 : Form {

		private Direct3D.Device device = null;
		private Limits2D screenLimits = new Limits2D();
		private Color backgroundColor = Color.LightBlue;
		private CameraViewType cameraView = CameraViewType.TopView;
		private Vector3 cameraLocation = new Vector3(0f, 0f, 50f);
		private Vector3 cameraUpVector = new Vector3(0f, 1f, 0f);
		static private List<Vector3> allPoints = new List<Vector3>();
		static private Direct3D.CustomVertex.PositionColored[] array = null;
		private int deltaX = 0;
		private int deltaY = 0;
		private int oldX = 0;
		private int oldY = 0;
		private Vector3 delta = new Vector3();
		private Vector3 angle_delta = new Vector3();
		private ObjectLimit objectLimits = new ObjectLimit();
		private float zoomX = 0;
		private float zoomY = 0;
		private float scaleKoef = 0.05f;
		private float deltaZoom = 1.0f;

		private struct Limits2D {
			public float Left, Right;
			public float Top, Bottom;
			public float Width() {
				return Math.Abs(Right - Left);
			}
			public float Height() {
				return Math.Abs(Top - Bottom);
			}
		}

		private struct ObjectLimit {
			public float Left, Right; // X-+
			public float Front, Back; // Y+-
			public float Top, Bottom; // Z+-
		}

		private enum CameraViewType {
			TopView,
			FrontView,
			RigthView,
			IsoView
		}

		private struct RectangleByPoint4 {
			public float X1, X2, X3, X4;
			public float Y1, Y2, Y3, Y4;
			public float Width() {
				return Math.Abs(X2 - X1);
			}
			public float Height() {
				return Math.Abs(Y2 - Y1);
			}
		}

		private RectangleByPoint4 rotateRectangle(RectangleByPoint4 rf, float angle) {
			RectangleByPoint4 result = new RectangleByPoint4();
			float radian = Direct3D.Geometry.DegreeToRadian(angle);
			float sn = (float)Math.Sin(radian);
			float cs = (float)Math.Cos(radian);
			float X0 = rf.X1 + rf.Width() / 2;
			float Y0 = rf.Y1 + rf.Height() / 2;

			RectangleByPoint4 tmp = rf;

			tmp.X1 = tmp.X1 - X0;
			tmp.Y1 = Y0 - tmp.Y1;
			tmp.X2 = tmp.X2 - X0;
			tmp.Y2 = Y0 - tmp.Y2;
			tmp.X3 = tmp.X3 - X0;
			tmp.Y3 = Y0 - tmp.Y3;
			tmp.X4 = tmp.X4 - X0;
			tmp.Y4 = Y0 - tmp.Y4;

			result.X1 = tmp.X1*cs + tmp.Y1*sn + X0;
			result.Y1 = Y0 + tmp.X1*sn - tmp.Y1*cs;
			result.X2 = tmp.X2*cs + tmp.Y2*sn + X0;
			result.Y2 = Y0 + tmp.X2*sn - tmp.Y2*cs;
			result.X3 = tmp.X3*cs + tmp.Y3*sn + X0;
			result.Y3 = Y0 + tmp.X3*sn - tmp.Y3*cs;
			result.X4 = tmp.X4*cs + tmp.Y4*sn + X0;
			result.Y4 = Y0 + tmp.X4*sn - tmp.Y4*cs;

			return result;
		}

		public Form1() {
			InitializeComponent();

			// for zooming
			pScreen.MouseWheel += new MouseEventHandler(pScreen_MouseWheel);
		}

		private void btnCreateDevice_Click(object sender, EventArgs e) {
			if (device == null)
				initializeGraphics();
			else
				labelLog.Text = "device already init!";
		}

		private void initScreenLimits() {
			screenLimits.Left = 0;
			screenLimits.Right = pScreen.Size.Width;
			screenLimits.Top = pScreen.Size.Height;
			screenLimits.Bottom = 0;

			zoomX = (pScreen.Size.Width * scaleKoef);
			zoomY = (pScreen.Size.Height * scaleKoef);

			deltaZoom = 1.0f;
		}

		public bool initializeGraphics() {
			Direct3D.PresentParameters prePars = new Direct3D.PresentParameters();
			prePars.Windowed = true;
			prePars.SwapEffect = Direct3D.SwapEffect.Discard;

			initScreenLimits();

			try {
				device = new Direct3D.Device(0, Direct3D.DeviceType.Hardware, pScreen, Direct3D.CreateFlags.SoftwareVertexProcessing, prePars);
				labelLog.Text = "DirectX initialized";
			} catch (DirectXException e) {
				labelLog.Text = "Error initialization DirectX " + e.ErrorString;
				MessageBox.Show(labelLog.Text);
				return false;
			}

			return true;
		}

		/*
		// NOTE: show support graphics mode if treeView1 on form 
		public void loadGraphics() {
			foreach (AdapterInformation ai in Manager.Adapters) {
				TreeNode root = new TreeNode(ai.Information.Description);
				TreeNode driverInfo = new TreeNode(
				string.Format("Driver information: {0} - {1}",
				ai.Information.DriverName,
				ai.Information.DriverVersion));

				root.Nodes.Add(driverInfo);

				TreeNode displayMode = new TreeNode(
				string.Format("Current Display Mode: {0}x{1}x{2}",
				ai.CurrentDisplayMode.Width,
				ai.CurrentDisplayMode.Height,
				ai.CurrentDisplayMode.Format));
				foreach(DisplayMode dm in ai.SupportedDisplayModes) {
					TreeNode supportedNode = new TreeNode(
					string.Format("Supported: {0}x{1}x{2}",
					dm.Width, dm.Height, dm.Format));
					displayMode.Nodes.Add(supportedNode);
				}

				root.Nodes.Add(displayMode);

				treeView1.Nodes.Add(root);
			}
		}*/

		private void setupCamera() {
			device.Transform.Projection = Matrix.OrthoOffCenterRH(
				screenLimits.Left, screenLimits.Right, screenLimits.Bottom, screenLimits.Top, 1111.0f, -555.0f);
				//screenLimits.Left, screenLimits.Right, screenLimits.Bottom, screenLimits.Top, -555.0f, 1111.0f);

			// add Camera
			device.Transform.View = Matrix.LookAtRH(
				cameraLocation,
				new Vector3(cameraLocation.X, cameraLocation.Y, 0f),
				cameraUpVector);

			// Light not need
			device.RenderState.Lighting = true;

			device.RenderState.Ambient = Color.White;

			// setup light
			device.Lights[0].Type = Direct3D.LightType.Directional;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Direction = new Vector3(0, 1, 1);
			//device.Lights[0].Commit(); // ???
			device.Lights[0].Enabled = true;

			// Do not hide the reverse side of a flat figure
			device.RenderState.CullMode = Direct3D.Cull.None;

			float initAngleA = 0;
			float initAngleB = 0;

			switch (cameraView) {
				case CameraViewType.FrontView:
					initAngleA = (float)(Math.PI / -2);
					break;
				case CameraViewType.RigthView:
					initAngleB = initAngleA = (float)(Math.PI / -2);
					break;
				case CameraViewType.IsoView:
					initAngleA = (float)(Math.PI / -4);
					initAngleB = -initAngleA;
					break;
				case CameraViewType.TopView:
				default:
					break;
			}

			device.Transform.World = Matrix.RotationYawPitchRoll(0f, initAngleA - angle_delta.Y, initAngleB - angle_delta.X);
		}

		private void rendering() {
			if (device == null)
				return;

			device.Clear(Direct3D.ClearFlags.Target, backgroundColor, 1.0f, 0);

			setupCamera();

			device.BeginScene();
			
			device.Material = new Direct3D.Material();

			device.VertexFormat = Direct3D.CustomVertex.PositionColored.Format;

			if (allPoints.Count > 0)
				device.DrawUserPrimitives(Direct3D.PrimitiveType.LineStrip, array.Length - 1, array);

			device.EndScene();

			device.BeginScene();

			Direct3D.Mesh mesh = Direct3D.Mesh.Sphere(device, 2f, 20, 20);
			Direct3D.Material sphereMaterial = new Direct3D.Material();
			sphereMaterial.Ambient = Color.Red;
			sphereMaterial.Diffuse = Color.Red;
			device.Material = sphereMaterial;
			mesh.DrawSubset(0);

			device.EndScene();

			device.Present();
		}

		private void pScreen_Paint(object sender, PaintEventArgs e) {
			rendering();
		}

		private void btnLoadCNC_Click(object sender, EventArgs e) {
			int iLine = 0;
			string s = "";
			Vector3 point;
			TextReader textFile = File.OpenText("Temp22.cnc");
			allPoints.Clear();
			allPoints.Add(new Vector3());
			while ((s = textFile.ReadLine()) != null) {
				iLine++;
				point = new Vector3();
				try {
					point.X = (float)Convert.ToDouble(s);
					objectLimits.Right = Math.Max(point.X, objectLimits.Right);
					objectLimits.Left = Math.Min(point.X, objectLimits.Left);
				} catch {
					s = "Error in line: " + iLine.ToString();
					break;
				}
				s = textFile.ReadLine();
				iLine++;
				try {
					point.Y = -(float)Convert.ToDouble(s);
					objectLimits.Front = Math.Max(point.Y, objectLimits.Front);
					objectLimits.Back = Math.Min(point.Y, objectLimits.Back);
				} catch {
					s = "Error in line: " + iLine.ToString();
					break;
				}
				s = textFile.ReadLine();
				iLine++;
				try {
					point.Z = (float)Convert.ToDouble(s);
					objectLimits.Top = Math.Max(point.Z, objectLimits.Top);
					objectLimits.Bottom = Math.Min(point.Z, objectLimits.Bottom);
				} catch {
					s = "Error in line: " + iLine.ToString();
					break;
				}
				allPoints.Add(point);
			}
			textFile.Close();
			s += "Loaded " + allPoints.Count.ToString() + " points.";
			s += "Limits: X[" + objectLimits.Left.ToString() + ", " + objectLimits.Right.ToString() + "], ";
			s += "Limits: Y[" + objectLimits.Back.ToString() + ", " + objectLimits.Front.ToString() + "], ";
			s += "Limits: Z[" + objectLimits.Bottom.ToString() + ", " + objectLimits.Top.ToString() + "]";
			labelLog.Text = s;
			array = new Direct3D.CustomVertex.PositionColored[allPoints.Count];

			for (int i = 0; i < allPoints.Count; i++) {
				array[i].Color = Color.Black.ToArgb();
				array[i].Position = allPoints[i];
			}
		}

		private float getNewPosition(float a, float b, float c) {
			float lenght = a - b;
			float indent = (c - Math.Abs(lenght)) / 2;
			if (Math.Abs(lenght) <= c)
				return (a - lenght - indent);
			return 0.0f;
		}

		private void addToListBox(RectangleByPoint4 r) {
			listBox1.Items.Add("X1 = " + r.X1.ToString() + ", Y1 = " + r.Y1.ToString());
			listBox1.Items.Add("X2 = " + r.X2.ToString() + ", Y2 = " + r.Y2.ToString());
			listBox1.Items.Add("X3 = " + r.X3.ToString() + ", Y3 = " + r.Y3.ToString());
			listBox1.Items.Add("X4 = " + r.X4.ToString() + ", Y4 = " + r.Y4.ToString());
		}

		private void renderNewCameraViewType(CameraViewType newView) {
			if (device == null)
				return;

			initScreenLimits();
			
			angle_delta.X = 0;
			angle_delta.Y = 0;
			cameraLocation.X = 0;
			cameraLocation.Y = 0;
			cameraView = newView;
			labelInfo.Text = "";
			
			float h, w;

			listBox1.Items.Clear();
			if (newView == CameraViewType.TopView) {
				w = objectLimits.Right - objectLimits.Left;
				h = objectLimits.Front - objectLimits.Back;
			} else if (newView == CameraViewType.FrontView) {
				w = objectLimits.Right - objectLimits.Left;
				h = objectLimits.Top - objectLimits.Bottom;
			} else if (newView == CameraViewType.RigthView) {
				w = objectLimits.Front - objectLimits.Back;
				h = objectLimits.Top - objectLimits.Bottom;
			} else { // IsoView
				RectangleByPoint4 r = new RectangleByPoint4();
				r.X1 = objectLimits.Left;
				r.X2 = objectLimits.Right;
				r.X4 = objectLimits.Left;
				r.X3 = objectLimits.Right;
				r.Y1 = objectLimits.Back;
				r.Y2 = objectLimits.Front;
				r.Y4 = objectLimits.Front;
				r.Y3 = objectLimits.Back;
				addToListBox(r);
				RectangleByPoint4 q = rotateRectangle(r, 45);
				addToListBox(q);
				float minX = q.X1, maxX = q.X1;
				minX = Math.Min(minX, q.X2);
				minX = Math.Min(minX, q.X3);
				minX = Math.Min(minX, q.X4);
				maxX = Math.Max(maxX, q.X2);
				maxX = Math.Max(maxX, q.X3);
				maxX = Math.Max(maxX, q.X4);
				w = maxX - minX;
				float minY = q.Y1, maxY = q.Y1;
				minY = Math.Min(minY, q.Y2);
				minY = Math.Min(minY, q.Y3);
				minY = Math.Min(minY, q.Y4);
				maxY = Math.Max(maxY, q.Y2);
				maxY = Math.Max(maxY, q.Y3);
				maxY = Math.Max(maxY, q.Y4);
				h = maxY - minY;
			}
			listBox1.Items.Add("w = " + w.ToString());
			listBox1.Items.Add("h = " + h.ToString());
			while ((h > (screenLimits.Top - screenLimits.Bottom)) || (w > (screenLimits.Right - screenLimits.Left))) {
				listBox1.Items.Add("screen.w = " + screenLimits.Width().ToString());
				listBox1.Items.Add("screen.h = " + screenLimits.Height().ToString());
				screenLimitsPlus();
			}
			if (deltaZoom == 1.0f) {
				while ((h < (screenLimits.Top - screenLimits.Bottom)) && (w < (screenLimits.Right - screenLimits.Left))) {
					listBox1.Items.Add("screen.w = " + screenLimits.Width().ToString());
					listBox1.Items.Add("screen.h = " + screenLimits.Height().ToString());
					screenLimitsMinus();
				}
				screenLimitsPlus();
			}
			listBox1.Items.Add("screen.w = " + screenLimits.Width().ToString());
			listBox1.Items.Add("screen.h = " + screenLimits.Height().ToString());
			if (newView == CameraViewType.TopView) {
				cameraLocation.X = getNewPosition(objectLimits.Right, objectLimits.Left, w) + (w - pScreen.Size.Width) / 2;
				cameraLocation.Y = getNewPosition(objectLimits.Front, objectLimits.Back, h) + (h - pScreen.Size.Height) / 2;
			} else if (newView == CameraViewType.FrontView) {
				cameraLocation.X = getNewPosition(objectLimits.Right, objectLimits.Left, w) + (w - pScreen.Size.Width) / 2;
				cameraLocation.Y = getNewPosition(objectLimits.Top, objectLimits.Bottom, h) + (h - pScreen.Size.Height) / 2;
			} else if (newView == CameraViewType.RigthView) {
				cameraLocation.X = getNewPosition(objectLimits.Front, objectLimits.Back, w) + (w - pScreen.Size.Width) / 2;
				cameraLocation.Y = getNewPosition(objectLimits.Top, objectLimits.Bottom, h) + (h - pScreen.Size.Height) / 2;
			} else { // IsoView
				// TODO: finish center view on object
			}
			labelInfo.Text += "Camera(" + cameraLocation.X.ToString() + ", " + cameraLocation.Y.ToString() + ")";
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
			if (e.Button == MouseButtons.Left) {
				deltaX = oldX - e.X;
				deltaY = oldY - e.Y;
				oldX = e.X;
				oldY = e.Y;
				labelLog.Text = "Mouse position: " + e.X.ToString() + ", " + e.Y.ToString() + ", delta: " + deltaX.ToString() + ", " + deltaY.ToString() +
					", Camera(" + cameraLocation.X.ToString() + ", " + cameraLocation.Y.ToString() + ")";

				cameraLocation.X += deltaX * deltaZoom;
				cameraLocation.Y -= deltaY * deltaZoom;

				rendering();
			}
			if (e.Button == MouseButtons.Right) {
				deltaX = oldX - e.X;
				deltaY = oldY - e.Y;
				oldX = e.X;
				oldY = e.Y;
				labelLog.Text = "Mouse position: " + e.X.ToString() + ", " + e.Y.ToString() + ", delta: " + deltaX.ToString() + ", " + deltaY.ToString() +
					", Camera(" + cameraLocation.X.ToString() + ", " + cameraLocation.Y.ToString() + ")";

				angle_delta.X -= deltaX * 0.01f;
				angle_delta.Y += deltaY * 0.01f;

				rendering();
			}
		}

		private void pScreen_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				oldX = e.X;
				oldY = e.Y;
			} else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
				oldX = e.X;
				oldY = e.Y;
			}
		}

		private void pScreen_MouseUp(object sender, MouseEventArgs e) {
		}

		private void screenLimitsPlus() {
			screenLimits.Left -= zoomX;
			screenLimits.Right += zoomX;
			screenLimits.Top += zoomY;
			screenLimits.Bottom -= zoomY;
			deltaZoom += scaleKoef*2;
		}

		private void screenLimitsMinus() {
			screenLimits.Left += zoomX;
			screenLimits.Right -= zoomX;
			screenLimits.Top -= zoomY;
			screenLimits.Bottom += zoomY;
			deltaZoom -= scaleKoef*2;
		}

		void pScreen_MouseWheel(object sender, MouseEventArgs e) {
			if (e.Delta > 0)
				screenLimitsPlus();
			else
				screenLimitsMinus();

			labelLog.Text = "Zoom: " + deltaZoom.ToString();

			rendering();
		}

		private void pScreen_Click(object sender, EventArgs e) {
			pScreen.Focus();
		}
	}
}
