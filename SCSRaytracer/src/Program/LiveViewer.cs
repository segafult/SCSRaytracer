//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Threading;

using SFML.Graphics;
using SFML.Window;

namespace RayTracer
{
    class LiveViewer
    {
        public SFML.Graphics.Image live_image;

        RenderWindow live_window;
        SFML.Graphics.Texture live_texture;
        Sprite live_sprite;
        Thread render_thread;

        double lookat = 0;
        World w;

        public LiveViewer(World worldref)
        {
            w = worldref;
        }

        public void set_up_liveview()
        {
			//Live view window
			live_window = new RenderWindow(new VideoMode((uint)w.vp.hres, (uint)w.vp.vres), "Live render view");
			live_window.Closed += new EventHandler(LiveviewOnClose);
            //live_window.KeyPressed += new EventHandler<KeyEventArgs>(LiveviewOnKeypress);
            //live_window.SetActive();

            //Initialize render targets
            live_image = new SFML.Graphics.Image((uint)w.vp.hres, (uint)w.vp.vres);
            live_texture = new SFML.Graphics.Texture(live_image);
            live_sprite = new Sprite(live_texture);

            //Dispatch thread for window.
            render_thread = new Thread(() => this.live_render_loop(live_sprite, live_texture, live_image));
            render_thread.Start();
        }

        private void live_render_loop(Sprite spr, SFML.Graphics.Texture tex, SFML.Graphics.Image img)
        {


			while (live_window!=null && !GlobalVars.should_close)
            {
				
                tex.Update(img);
				if(live_window.IsOpen)
                	live_window.Draw(spr);
				if(live_window.IsOpen)
					live_window.Display();
            }
				
			return;
        }
        private void LiveviewOnClose(Object sender, EventArgs e)
        {
            Window window = (Window)sender;
			GlobalVars.should_close = true;
			window.SetActive (false);
            window.Close();
        }
        private void LiveviewOnKeypress(Object sender, EventArgs e)
        {
            w.camera.setLookat(new Point3D(lookat += 10, 0, 0));
            w.camera.compute_uvw();
        }
        public Thread get_thread()
        {
            return render_thread;
        }
        public void poll_events()
        {
            live_window.DispatchEvents();
        }
    }
}
