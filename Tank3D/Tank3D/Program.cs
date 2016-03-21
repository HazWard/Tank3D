using System;

namespace AtelierXNA
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MenuPrincipal game = new MenuPrincipal())
            {
                game.Run();
            }
        }
    }
#endif
}