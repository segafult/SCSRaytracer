//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Threading;

using SFML.Graphics;
using SFML.Window;

namespace SCSRaytracer
{
    sealed class LiveViewer
    {
        private SFML.Graphics.Image _liveImage;
        private Thread _renderThread;
        private RenderWindow _liveWindow;
        private SFML.Graphics.Texture _liveTexture;
        private Sprite _liveSprite;
        private float _lookAt = 0;
        private World _world;

        // accessors
        public SFML.Graphics.Image LiveImage
        {
            get
            {
                return _liveImage;
            }
            set
            {
                _liveImage = value;
            }
        }
        public Thread RenderThread
        {
            get
            {
                return _renderThread;
            }
        }

        public LiveViewer(World worldRef)
        {
            _world = worldRef;
        }

        public void SetUpLiveView()
        {
			//Create new rendering window and add event handling for when the window is closed
			_liveWindow = new RenderWindow(new VideoMode((uint)_world.CurrentViewPlane.HorizontalResolution, (uint)_world.CurrentViewPlane.VerticalResolution), "Live render view");
			_liveWindow.Closed += new EventHandler(LiveViewOnClose);

            //Initialize render targets
            _liveImage = new SFML.Graphics.Image((uint)_world.CurrentViewPlane.HorizontalResolution, (uint)_world.CurrentViewPlane.VerticalResolution);
            _liveTexture = new SFML.Graphics.Texture(_liveImage);
            _liveSprite = new Sprite(_liveTexture);

            //Dispatch thread for window.
            _renderThread = new Thread(() => this.LiveRenderLoop(_liveSprite, _liveTexture, _liveImage));
            _renderThread.Priority = ThreadPriority.Normal;
            _renderThread.Start();
        }

        /// <summary>
        /// Method executed by main rendering thread.
        /// </summary>
        /// <param name="spr">Sprite reference</param>
        /// <param name="tex">Texture reference</param>
        /// <param name="img">Image reference</param>
        private void LiveRenderLoop(Sprite spr, SFML.Graphics.Texture tex, SFML.Graphics.Image img)
        {
            // Spin loop. Loop until window should be closed, update texture, draw and display it based on current
            // status of rendering
			while (_liveWindow != null && !GlobalVars.should_close)
            {
                tex.Update(_world.RenderImage);
				if(_liveWindow.IsOpen)
                	_liveWindow.Draw(spr);
				if(_liveWindow.IsOpen)
					_liveWindow.Display();
            }
				
        }

        /// <summary>
        /// Event handling for live view window closed
        /// </summary>
        /// <param name="sender">Window that sent the message</param>
        /// <param name="e">Event arguments</param>
        private void LiveViewOnClose(Object sender, EventArgs e)
        {
            Window window = (Window)sender;
			GlobalVars.should_close = true;
			window.SetActive (false);
            window.Close();
        }

        /// <summary>
        /// Event handling for keypresses within live render window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiveViewOnKeyPress(Object sender, EventArgs e)
        {
            _world.Camera.LookAt = new Point3D(_lookAt += 10, 0, 0);
            _world.Camera.compute_uvw();
        }

        /// <summary>
        /// Poll events for window
        /// </summary>
        public void PollEvents()
        {
            _liveWindow.DispatchEvents();
        }
    }
}
