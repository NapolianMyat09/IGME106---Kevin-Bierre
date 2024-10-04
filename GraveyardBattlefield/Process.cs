using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using System.Transactions;

namespace GraveyardBattlefield
{
    /*
      * Project: Graveyard BattleField
      * Names: Tracy Chun, Jason Wang, Napolian Myat
      * Class: Process
      * Purpose: - methods and fields that deal with processing data 
      * 
      * Updates:
      * 
      */
    class Process
    {

        //fields
        private static KeyboardState previousKbState = Keyboard.GetState();
        private static MouseState previousMState = Mouse.GetState();
        public static KeyboardState PreviousKbState { get; set; }
        /// <summary>
        /// Checks if the key processed through is pressed by checking keyboard states
        /// </summary>
        /// <param name="key"></param>
        /// <param name="currentKbState"></param>
        public static bool SingleKeyPress(KeyboardState currentKbState, Keys key)
        {
            if (currentKbState.IsKeyDown(key) && previousKbState.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MouseClick(MouseState currentMState, Rectangle buttonLocation)
        {
            bool success = false; //bool to see if a button was clicked
            if (currentMState.LeftButton == ButtonState.Released && previousMState.LeftButton == ButtonState.Pressed)
                //if mouse was clicked
            {
                if (buttonLocation.Contains(currentMState.Position))//and if mouse was clicking on button
                {
                    success = true; //return true
                }
                else
                {
                    success = false; //otherwise button was not clicked
                }
            }
            previousMState = currentMState; //update previous mouse state
            return success; //return if button was clicked
        }

        /// <summary>
        /// load text file with data in a specific format 
        /// save all the information in the correct places
        /// </summary>
        /// <param name="path"></param>
        public void LoadFile(string path)
        {
            StreamReader output = null;
            {
                try
                {
                    //check for the file path
                    output = new StreamReader(path);

                    //loop through data in the text file and split apart with console writelines
                    string line = null;
                    List<string> lines = new List<string>();
                    int i = 0;
                    while ((line = output.ReadLine()) != null)
                    {
                        lines[i] = output.ReadLine();
                        i++;
                    }
                    /*
                     * put each item in the respective field using the list
                     */
                    output.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There seems to an error loading the text file: " + ex.Message);
                }
            }
        }
    }
}
