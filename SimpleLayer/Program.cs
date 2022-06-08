using System;
using SimpleLayer;
using SimpleLayer.GameEngine;

namespace SimpleLayer;

    class Program
    {
        private static readonly Game Game = new Game();

        static void Main(string[] args)
        {
            Game.Run();
        }
    }

