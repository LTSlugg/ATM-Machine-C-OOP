using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATM_Machine
{
    /*
     *  ATM Code with Object Oriented Programming
     *  Made by Luis Troncoso
     */

    internal class Program
    {
        static void Main(string[] args)
        {
            //Declared Variables
            int userEntry;
            bool isAuthenticated = false;


            ATMachine atMachine = new ATMachine();                  //New Instance of the ATM Class
            UserAccount[] userAccounts = new UserAccount[3];        //User Account Class Array to track all the users
            UserAccount currentUser;                                //The current user when authenticated


            //Calling Functions
            Initialize();
            AuthenticateUser();
            


            //Functions
            //Initializes all the necessary stuff (IE Sets up the UA Array with Information)
            void Initialize() {

                Console.WriteLine("Thank you for Banking with us at BDZ!");                       //Initial Welcome statement

                //Sets each object in each array index to a user account
                for (int i = 0; i < userAccounts.Length; i++)
                {
                    userAccounts[i] = new UserAccount();
                }

                //Sets each user account in the array with the correct information
                userAccounts[0].SetUpAccount("Cole Ruppel", 0327, 1000, "704 Fake Ave, Faketown, FakeState 02132");
                userAccounts[1].SetUpAccount("Sarah Leevee", 4321, 1500, "1200 Fake Blvd, Faketown, FakeState 02132");
                userAccounts[2].SetUpAccount("Luis Troncoso", 1234, 300, "2870 Fake Ave, Fakishcity, FakeState 02132");
            }

            //Initial While loop to keep the program confined to the PIN Entry screen until a success has been determined
            void AuthenticateUser()
            {
                while (!isAuthenticated)
                {
                    Console.WriteLine("Please input your 4-Digit Numerical Pin: ");

                    if (int.TryParse(Console.ReadLine(), out userEntry))                      //Attemps to Parse the String to a Integer and assign it to userEntry
                    {
                        CheckPIN(userEntry);                                                //Checks the Pin
                    }
                    else { MessageHandler.FailedMessages(1); }
                }
            }

            //Method to check if the pin is correct by searching through the PINS in the UA (User Accounts) Array
            void CheckPIN(int userPIN)
            {
                for (int i = 0; i < userAccounts.Length; i++)                                       //For loop to cycle through the Array
                {
                    if (userPIN == userAccounts[i].userPIN)                                     //Searches through the UA (User Accounts) Array to find a matching PIN
                    {
                        Console.WriteLine("Welcome back " + userAccounts[i].userName);           //Prints a welcome on a successful PIN Check
                        System.Threading.Thread.Sleep(1000);                               //Wait 1 Seconds then continue
                        isAuthenticated = true;
                        currentUser = userAccounts[i];
                        
                        System.Threading.Thread.Sleep(1000);                               //Wait 1 Seconds then continue
                   
                        atMachine.MainLoop(currentUser);                                  //Starts the Main Loop of the ATMachine after Authentication
                    }
                    else if (i >= userAccounts.Length - 1 && !isAuthenticated)          //Checks to Ensure the Iteration is done with by checking i against the length of the Array - 1 due to explicity
                    {
                        MessageHandler.FailedMessages(2);
                    }
                }
            }
        }
    }


    /*
     *  The ATM Machine itself
     */
    public class ATMachine()
    {

        //Declared Variables
        public bool exitApplication = false;

        //Functions
        //The Main Loop of the ATMachine Class
        public void MainLoop(UserAccount currentUser)
        {
            while (!exitApplication)
            {
                Console.WriteLine("********Main Menu********");
                Console.WriteLine("**Please choose one of the options**");
                Console.WriteLine( "\nA. Check Current Balance" +
                    "               \nB. Deposit Amount" +
                    "               \nC. Withdraw Amount" +
                    "               \nD. Transaction History" +
                    "               \nE. Exit");
                

                switch (Console.ReadLine().ToLower()) //Lowers the case of the string
                {
                    case "a":
                        CheckBalance(currentUser);
                        break;

                    case "b":
                        Deposit(currentUser);
                        break;

                    case "c":
                        Withdraw(currentUser);
                        break;

                    case "d":
                        TransactionHistory(currentUser);
                        break;

                    case "e":
                        ExitThisApplication();
                        break;

                    default:
                        MessageHandler.FailedMessages(1);
                        break;
                }
            }

        }

        //Allows the user to check their current balance
        public void CheckBalance(UserAccount currentUser)
        {
            Console.WriteLine("Your current balance is: " + currentUser.currentBalance + "$\n");
        }

        //Allows the user to Deposit money
        public void Deposit(UserAccount currentUser)
        {
            float depositAmount;
            if (currentUser.transactionCount < currentUser.transactionLimit)
            {
                Console.WriteLine("How much would you like to deposit?");                                       //Alerts the user   

                if (float.TryParse(Console.ReadLine(), out depositAmount))                                      //Attempts to check if the input can be converted to a variable
                {
                    currentUser.transactionCount++;                                                             //Add to the Transaction Count
                    currentUser.currentBalance += depositAmount;                                                   //Do the Math to add the deposit to the users balance
                    currentUser.TransactionCapture(true, depositAmount);                                        //Calls to capture the transaction

                    Console.WriteLine("You deposited " + depositAmount + "$");                                  //Alerts the user   
                    Thread.Sleep(1000);
                    Console.WriteLine("Your new balance is: " + currentUser.currentBalance + "$");                 
                    Thread.Sleep(2000);
                }
                else
                {
                    MessageHandler.FailedMessages(1);                                                           //Throws this message on the incorrect input
                }
            }
            else
            {
                MessageHandler.FailedMessages(4);
            }

        }

        //Allows the user to Withdraw Money
        public void Withdraw(UserAccount currentUser)
        {
            float withdrawAmount = 20;                                                      //User can only withdraw 20$ per withdraw attempt


            if (currentUser.currentBalance >= withdrawAmount && currentUser.transactionCount < currentUser.transactionLimit)
            {
                currentUser.transactionCount++;
                Console.WriteLine("You withdrew 20$");                                      //Alerts the user

                currentUser.currentBalance -= withdrawAmount;                              //Adjusts the balance
                currentUser.TransactionCapture(false, withdrawAmount);                  //Sends a Capture call for the Transaction History

                Thread.Sleep(1000);

                Console.WriteLine("Your new balance is: " + currentUser.currentBalance + "$");                 //Alerts the user   

                Thread.Sleep(3000);                                                                        //Pause for 3 Seconds
            }
            else if (currentUser.transactionCount >= currentUser.transactionLimit)
            {
                MessageHandler.FailedMessages(4);                                                       //Throws Message on Transaction Limit Reached
            }
            else
            {
                MessageHandler.FailedMessages(3);                                                       //Throws error message on insufficient funds
            }   
                                                                 
        }

        //Shows the users last 5 Transactions
        public void TransactionHistory(UserAccount currentUser)
        {
            currentUser.TransactionPrint();     //Prints the current users transaction history
        }


        //Handles Exiting the Application
        public void ExitThisApplication()
        {
            Console.WriteLine("Thank you for banking with us at BDZ, we always appreciate your support!");
            Thread.Sleep(5000);
            exitApplication = true;
            Environment.Exit(0);
        }
    }


    /* Keeps the users information
     * Users Name, Balance, PIN, Address
     */
    public class UserAccount
    {
        //Declared Variables
        public string userName { get; private set; }
        public int userPIN { get; private set; }
        public float currentBalance { get; set; }
        public string currentAddress { get; private set; }
        
        public int transactionCount = 0;                    //Used to Track the Amount of Transactions
        public int transactionLimit = 5;
        
        public string[] transactionType = new string[10];                        //Tracks the Transaction Type
        public float[] transactionAmount = new float[10];                       //Tracks the Transaction Amount


        //Functions
        //Sets up the account information
        public void SetUpAccount(string UserName, int UserPin, float UserBalance, string UserAddress)
        {
            userName = UserName;
            userPIN = UserPin;
            currentBalance = UserBalance;
            currentAddress = UserAddress;
        }


        //Captures the Transactions Informations - Type and Amount
        public void TransactionCapture(bool isDeposit, float amount)
        {
            //Captures if its a Deposit or Withdrawal
            if (isDeposit)
            {
                transactionType[transactionCount] = DateTime.Now.ToString("MM/dd/yyyy, HH:mm") +  " - Deposited -";
            }
            else if (!isDeposit)
            {
                transactionType[transactionCount] = DateTime.Now.ToString("MM/dd/yyyy, HH:mm") + " - Withdrawal -";           
            }

            transactionAmount[transactionCount] = amount;                       //Captures the Amount on an Array
        }


        //Prints the Transaction Type and Amount in order from the last being the first
        public void TransactionPrint()
        {
            Console.WriteLine("*****Transaction History*****");   

            for (int i = transactionCount; i > 0; i--)
            {
                Console.WriteLine(transactionType[i] + " " + transactionAmount[i] + "$");
            }
        }
    }


    /* Message Handler
     * Handlers showing error messages
     */
    public class MessageHandler()
    {
        //A Message and Timer when Authentication has Failed
        public static void FailedMessages(int messageChoice)
        {
            switch (messageChoice)
            {
                case 1:
                    Console.WriteLine("Incorrect Entry, Please try Again.");                          //Throws Error if Incorrect
                    System.Threading.Thread.Sleep(2000);                                             //Wait 2 Seconds then continue
                    break;

                case 2:
                    Console.WriteLine("Failed to Authenticate User, Please try Again.");             //Throws Error if failed to authenticate
                    System.Threading.Thread.Sleep(2000);                                            //Wait 2 Seconds then continue
                    break;

                case 3:
                    Console.WriteLine("Unable to withdraw due to insufficent funds, returning to the Main Menu");                //Throws error if insufficent funds
                    System.Threading.Thread.Sleep(2000);                                            //Wait 2 Seconds then continue
                    break;
                case 4:
                    Console.WriteLine("You reached your daily transaction limit (5), returning to the Main Menu");
                    break;
            }

        }
    }
}
