//using System.Threading;

using System.Linq.Expressions;

namespace watersim_2
{
    internal class Program
    {
        public static int SIZE;

        public static int[] range = {30, 240}; //first is min and second is max of SIZE
        public static int[] size_range = {60, 75, 95, 120, 160};// set font size by comparing this array's elemnts to SIZE
        public static short[] font_size = {14, 12, 10, 8, 6 };

        //public static int[,] world_array = new int[SIZE,SIZE]; if you want to use threaded mode
        //also set SIZE to a value and allow TwSim and unallow start_sc() and be aware that
        // 14px->67 max  12px->79 max  10px->96max  8px->122max  6px->164max  is the ration to chose the SIZE
        //and you need to manually set the console's font size
        static void Main(string[] args)
        {
            start_sc();
            //TwSim(world_array);
            Console.SetCursorPosition(0, SIZE);

            Console.ReadKey();
        }

        static void start_sc() {
            Console.Title = "Menu";
            Console.SetWindowSize(80,30);
            ConsoleHelper.SetCurrentFont("Consolas", 14);
            bool pass = false;
            while (!pass)
            {
                Console.WriteLine($"Specify the size of the map by giving it a number between {range[0]} and {range[1]}");
                Console.WriteLine("It will create a square like map and a window fitting for it too");
                Console.Write("The size: ");
                try{
                    SIZE = int.Parse(Console.ReadLine());
                    if (range[1] >= SIZE && SIZE >= range[0]) { pass = true; }
                    Console.Clear();
                    Console.WriteLine("Your number is bigger or smaller than the accaptable range please chose again");
                }
                catch (Exception e){ Console.Clear(); Console.WriteLine("Not accaptable character please chose again"); }
            }
            int[,] world_array = new int[SIZE, SIZE];
            Setup();
            wGen(world_array);
            fdraw(world_array);
            wSim(world_array);
        }

        static void Setup() {
            Console.Clear();
            //comparing SIZE to array's elements
            for (int i = 0; i < size_range.Length; i++)
            {
                if (i == 0)
                {
                    if (size_range[0] > SIZE && SIZE >= range[0])
                    {
                        ConsoleHelper.SetCurrentFont("Consolas", font_size[i]);//stackoverflow: https://stackoverflow.com/a/62763047/20400339 thanks for it
                    }
                }
                else if (size_range[i-1] < SIZE && size_range[i] >= SIZE)
                {
                    ConsoleHelper.SetCurrentFont("Consolas", font_size[i]);//stackoverflow: https://stackoverflow.com/a/62763047/20400339
                }
            }
            
            int diff = 7; // adds a plusz to the size for better look, it is recomended that value is not changed as if smaller than 6
                          // it is possible to not see everything
            Console.SetWindowSize(2*SIZE + diff, SIZE + diff);
            Console.Title = "Water Simulator";
        }


        //first draw
        static void fdraw(int[,] world) {
            for (int i = 0; i < world.GetLength(0); i++){
                for (int j = 0; j < world.GetLength(1); j++) {
                    if (world[i, j] == 1 || world[i, j] == 3) { Console.BackgroundColor = ConsoleColor.Gray; }
                    else if (world[i, j] == 0) { Console.BackgroundColor = ConsoleColor.Blue; }
                    else Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("  ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
        }

        /*******************************************************************************************************
         * first draw methode, slow as redraws hole line not just one element
        static void draw(int i) {
            Console.SetCursorPosition(0, i);
            for (int j = 0;j < world.GetLength(0); j++){
                if (world[i, j] == 1 || world[i, j] == 3) { Console.BackgroundColor = ConsoleColor.Gray; }
                else if (world[i, j] == 0) { Console.BackgroundColor = ConsoleColor.Blue; }
                else Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write("  ");
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }
        *********************************************************************************************************/

        //generates random world
        static void wGen(int[,] world) {
            Random rnd = new Random();
            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (i == world.GetLength(0)-1 || j == 0 || j == world.GetLength(1)-1) { world[i, j] = 2; }
                    else if (i == 0) { world[i, j] = 0; }
                    else world[i, j] = rnd.Next(1, 4);
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }


        //simulate the possible paths that water can go to
        static void wSim(int[,] world) {
            for (int w = 0; w < SIZE; w++) {  //checks other possibilities after update in data
                for (int i = 1; i < world.GetLength(0); i++) {  //we do not need to check the first line
                    for (int j = 1; j < world.GetLength(1)-1; j++) {  // we do not need to check first and last element as they are DarkGrey always
                        //checks if data is 1 or 3 (gery) and can be 0(water), can water fall down by 1
                        //if (i > 0 && world[i - 1, j] == 0 && (world[i, j] == 1 || world[i, j] == 3)) { world[i, j] = 0; draw(i); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && i > 0 && world[i - 1, j] == 0) { world[i, j] = 0; Tdraw(i, j, world); }


                        //checks if data is 1 or 3(grey) and possible to go to left by 1
                        //if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j + 1] == 2) { world[i, j] = 0; draw(i); }
                        //if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j + 1] == 2) { world[i, j] = 0; draw(i); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j + 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j + 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }

                        //checks if data is 1 or 3(grey) and possible to go right by 1
                        //if (world[i, j] == 1 && world[i, j - 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j - 1] == 2) { world[i, j] = 0; draw(i); }
                        //if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j - 1] == 0 && world[i + 1, j - 1] == 2) { world[i, j] = 0; draw(i); }
                        if (world[i, j] == 1 && world[i, j - 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j - 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j - 1] == 0 && world[i + 1, j - 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                    }
                }
            }
        }

        //originally for threaded mode but runs faster than draw, as only draws the necessary element
        static void Tdraw(int i, int j, int[,] world) {
            Console.SetCursorPosition(2*j, i);  //might be confusing as it asks for x coord first and y after that
            if (world[i, j] == 1 || world[i, j] == 3) { Console.BackgroundColor = ConsoleColor.Gray; }
            else if (world[i, j] == 0) { Console.BackgroundColor = ConsoleColor.Blue; }
            else Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write("  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        //--------------------------------------------------------------------------------------------------------------------------------//
        //checks possibilities//

        static void check_down(int[,] world) {
                for (int i = 1; i < world.GetLength(0); i++)
                {
                    for (int j = 1; j < world.GetLength(1)-1; j++)
                    {
                        if ((world[i, j] == 1 || world[i, j] == 3) && i > 0 && world[i - 1, j] == 0) { world[i, j] = 0; Tdraw(i, j, world); }
                    }
                }
        }
        static void check_right(int[,] world) {
                for (int i = 1; i < world.GetLength(0); i++)
                {
                    for (int j = 1; j < world.GetLength(1)-1; j++)
                    {
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j - 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j - 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j - 1] == 0 && world[i + 1, j - 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                    }
                }
        }

        static void check_left(int[,] world) {
                for (int i = 1; i < world.GetLength(0); i++)
                {
                    for (int j = 1; j < world.GetLength(1)-1; j++)
                    {
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j] == 2 && world[i + 1, j + 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                        if ((world[i, j] == 1 || world[i, j] == 3) && world[i, j + 1] == 0 && world[i + 1, j + 1] == 2) { world[i, j] = 0; Tdraw(i, j, world); }
                    }
                }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        //threaded mode, not sure if it runs faster
        async static Task TwSim(int[,] world) {
            int diff = 5; // adds a plusz to the size for better look
            Console.SetWindowSize(2 * SIZE + diff, SIZE + diff);
            Console.Title = "Water Simulator";
            for (int i = 0; i < SIZE; i++)
            {
                await Task.Run(() => check_down(world));
                await Task.Run(() => check_left(world));
                await Task.Run(() => check_right(world));
            }
        }
    }
}
