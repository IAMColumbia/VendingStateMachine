using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FiniteStateMachine
{
    public class VendingMachineTransition
    {
        public VendingMachine.State State;
        public VendingMachine.Input Input;
        public VendingMachine.Output Output;
        public VendingMachine.State OutputState;

        public VendingMachineTransition(VendingMachine.State state, VendingMachine.Input input, VendingMachine.Output output, VendingMachine.State outputState = VendingMachine.State.On)
        {
            State = state;
            Input = input;
            Output = output;
            OutputState = outputState;
        }
    }
    public class VendingMachine
    {
        public enum State { Off, On }
        public enum Input { InsertQuarter, PurchaseGum, PurchaseGranola, PowerButton }
        public enum Output { None = -2, Quarter = -1, Gum_50Cents = 50, Granola_75Cents = 75 }
        //encoding the balance requirement in the enum is kinda funny ^

        private State state = State.On;
        private Input? input;
        private double balance;
        private List<VendingMachineTransition> Transitions = new List<VendingMachineTransition>();

        public VendingMachine()
        {
            //Setup Transitions
            foreach (Input i in Enum.GetValues(typeof(Input)))
            {
                if (i != Input.PowerButton)
                { Transitions.Add(new VendingMachineTransition(State.Off, i, Output.None, State.Off)); }
                else
                { Transitions.Add(new VendingMachineTransition(State.Off, Input.PowerButton, Output.None, State.On)); }
            }
            Transitions.Add(new VendingMachineTransition(State.On, Input.InsertQuarter, Output.None));
            Transitions.Add(new VendingMachineTransition(State.On, Input.PurchaseGum, Output.Gum_50Cents));
            Transitions.Add(new VendingMachineTransition(State.On, Input.PurchaseGranola, Output.Granola_75Cents));
            Transitions.Add(new VendingMachineTransition(State.On, Input.PowerButton, Output.None, State.Off));

            while(true)
            {
                DisplayStatus();
                GetInput();
                TransitionAndOutput();
            }
        }

        private void DisplayStatus()
        {
            Console.Clear();
            Console.WriteLine(" = VendingMachine = ");
            Console.WriteLine($" State: {state.ToString()}");
            Console.WriteLine($" Balance: {balance}");
            Console.WriteLine("--------------------");
        }

        private void GetInput()
        {
            input = null;

            while(!input.HasValue)
            {
                Console.WriteLine("Select an Input:");
                for(int i = 0; i < Enum.GetNames(typeof(Input)).Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {Enum.GetNames(typeof(Input))[i]}");
                }
                
                string? inputText = Console.ReadLine();

                if(inputText == null)
                { continue; }

                int inputInt;
                
                if(int.TryParse(inputText, out inputInt))
                { input = (Input)inputInt - 1; }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You selected: " + input.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void TransitionAndOutput()
        {
            VendingMachineTransition? transition = Transitions.Where(s => s.State == state && s.Input == input.Value).FirstOrDefault();
            if(transition == null)
            { throw new Exception("Transition is null"); }

            if(state != transition.OutputState)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Changed State to " + transition.OutputState.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
            state = transition.OutputState;

            Output output = transition.Output;

            if (transition.Input == Input.InsertQuarter && transition.State != State.Off)
            { balance += 0.25; }
            
            if ((int)output > balance * 100) //not enough money
            { output = Output.None; }
            else if((int)output > 0) //purchased something
            { balance -= (int)output / 100f; }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Output: " + output.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
    }
}
