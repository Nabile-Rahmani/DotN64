using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace DotN64.Desktop.SDL
{
    using RCP;
    using static RCP.RealityCoprocessor.VideoInterface;

    internal class Window : IVideoOutput, IDisposable
    {
        #region Fields
        private readonly IntPtr window;
        private readonly Nintendo64 nintendo64;
        private readonly Dictionary<ControlRegister.PixelSize, uint> pixelFormats = new Dictionary<ControlRegister.PixelSize, uint>
        {
            [ControlRegister.PixelSize.RGBA5553] = SDL_PIXELFORMAT_RGBA5551,
            [ControlRegister.PixelSize.RGBA8888] = SDL_PIXELFORMAT_RGBA8888
        };
        private IntPtr renderer, texture;
        private VideoFrame lastFrame;
        private bool isDisposed;
        #endregion

        #region Properties
        public string Title
        {
            get => SDL_GetWindowTitle(window);
            set => SDL_SetWindowTitle(window, value);
        }

        public Point Position
        {
            get
            {
                var position = new Point();
                SDL_GetWindowPosition(window, out position.X, out position.Y);

                return position;
            }
            set => SDL_SetWindowPosition(window, value.X, value.Y);
        }

        public Point Size
        {
            get
            {
                var size = new Point();
                SDL_GetWindowSize(window, out size.X, out size.Y);

                return size;
            }
            set => SDL_SetWindowSize(window, value.X, value.Y);
        }

        public bool IsFullScreen
        {
            get => (SDL_GetWindowFlags(window) & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
            set => SDL_SetWindowFullscreen(window, value ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0); // Could also go exclusive fullscreen with desktop bounds.
        }

        public bool IsBorderless
        {
            get => (SDL_GetWindowFlags(window) & (uint)SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0;
            set => SDL_SetWindowBordered(window, !value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }
        #endregion

        #region Constructors
        public Window(Nintendo64 nintendo64, string title = null, Point? position = null, Point? size = null)
        {
            this.nintendo64 = nintendo64;
            position = position ?? new Point(SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            size = size ?? new Point(640, 480);
            window = SDL_CreateWindow(title ?? nameof(DotN64), position.Value.X, position.Value.Y, size.Value.X, size.Value.Y, SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            renderer = SDL_CreateRenderer(window, SDL_GetWindowDisplayIndex(window), SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        }

        ~Window()
        {
            Dispose(false);
        }

        static Window()
        {
            SDL_Init(SDL_INIT_VIDEO);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => SDL_Quit();
        }
        #endregion

        #region Methods
        private void PollEvents()
        {
            while (SDL_PollEvent(out var sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        nintendo64.PowerOff();
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        switch (sdlEvent.key.keysym.sym)
                        {
                            case SDL_Keycode.SDLK_ESCAPE:
                                nintendo64.PowerOff();
                                break;
                            case SDL_Keycode.SDLK_PAUSE:
                                nintendo64.Debugger = nintendo64.Debugger ?? new Diagnostics.Debugger(nintendo64);
                                break;
                            case SDL_Keycode.SDLK_r: // TODO: Reset.
                                break;
                            case SDL_Keycode.SDLK_f:
                                IsFullScreen = !IsFullScreen;
                                break;
                        }
                        break;
                }
            }
        }

        public unsafe void Draw(VideoFrame frame, RealityCoprocessor.VideoInterface vi, RDRAM ram)
        {
            PollEvents();

            if (frame.Size <= ControlRegister.PixelSize.Reserved || frame.Width <= 0 || frame.Height <= 0) // Do nothing on Blank or Reserved frame.
            {
                // Might want to clear the screen.
                SDL_RenderPresent(renderer);
                return;
            }

            if (frame != lastFrame)
            {
                SDL_DestroyTexture(texture);
                texture = SDL_CreateTexture(renderer, pixelFormats[frame.Size], (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, frame.Width, frame.Height);
                lastFrame = frame;
            }

            SDL_LockTexture(texture, IntPtr.Zero, out var pixels, out var pitch);

            // TODO: This should be moved to the VI itself, which would call VideoOutput methods instead.
            for (vi.CurrentVerticalLine = 0; vi.CurrentVerticalLine < vi.VerticalSync; vi.CurrentVerticalLine++) // Sweep all the way down the screen.
            {
                if (vi.CurrentVerticalLine < vi.VerticalVideo.ActiveVideoStart || vi.CurrentVerticalLine >= vi.VerticalVideo.ActiveVideoEnd) // Only scan active lines.
                    continue;

                //var line = (ushort)(((vi.CurrentVerticalLine - vi.VerticalVideo.ActiveVideoStart) >> 1) * (float)vi.VerticalScale.ScaleUpFactor / (1 << 10));
                var line = (ushort)((vi.CurrentVerticalLine - vi.VerticalVideo.ActiveVideoStart) / (float)(vi.VerticalVideo.ActiveVideoEnd - vi.VerticalVideo.ActiveVideoStart) * frame.Height);
                var offset = pitch * line;

                if (!BitConverter.IsLittleEndian || frame.Size != ControlRegister.PixelSize.RGBA5553)
                    Marshal.Copy(ram.Memory, (int)vi.DRAMAddress + offset, pixels + offset, pitch);
                else
                {
                    fixed (byte* rdram = &ram.Memory[(int)vi.DRAMAddress])
                    {
                        for (int end = offset + pitch; offset < end; offset += 4)
                        {
                            ((ushort*)(pixels + offset))[0] = ((ushort*)(rdram + offset))[1];
                            ((ushort*)(pixels + offset))[1] = ((ushort*)(rdram + offset))[0];
                        }
                    }
                }
            }

            SDL_UnlockTexture(texture);
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero);
            SDL_RenderPresent(renderer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
