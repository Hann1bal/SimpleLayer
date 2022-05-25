using System;
using SimpleLayer;
using SimpleLayer.GameEngine;

namespace SimpleLayer;

    class Program
    {
        private readonly Game _game;

        static void Main(string[] args)
        {
            var game = new Game(); 
            game.Run();
        }
    }

