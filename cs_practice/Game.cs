using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Text.Json;



namespace GameController {

    public record struct PitchHit {
        public string name {get; init;}
        public int strike {get; init;}
        public int ball {get; init;}
        public int fly {get; init;}
        public int first {get; init;}
        public int second {get; init;}
        public int third {get; init;}
    }

    class BaseballGame {
        int user = 0, comp = 0;
        int inning = 0, outs = 0, balls = 0, strikes = 0;
        bool pitching = true;
        bool first = false, second = false, third = false;
        List<PitchHit> pitches = [];
        List<PitchHit> hits = [];

        /// <summary>
        /// Method <c>getPitches_Hits</c> retrieves the pitching and hitting probabilties from pitchOdds.json and hitOdds.json in the Configs folder
        /// </summary>
        void getPitches_Hits() {
            var pitchesList = new List<PitchHit>();
            
            using(StreamReader r = new StreamReader("Configs/pitchOdds.json")) {
                string json = r.ReadToEnd();
                pitchesList = JsonSerializer.Deserialize<List<PitchHit>>(json);
            }

            if(pitchesList != null && pitchesList.Count() > 0) {
                Console.WriteLine("Pitch Types Loaded:");
                foreach(var pitchType in pitchesList) Console.WriteLine(pitchType.name);
            }
            else {
                Console.WriteLine("There are {0} total records");
            }

            var hitsList = new List<PitchHit>();
            using(StreamReader r = new StreamReader("Configs/hitOdds.json")) {
                string json = r.ReadToEnd();
                hitsList = JsonSerializer.Deserialize<List<PitchHit>>(json);
            }

            if(hitsList != null && hitsList.Count() > 0) {
                Console.WriteLine("\nHit Types Loaded:");
                foreach(var hitType in hitsList) Console.WriteLine(hitType.name);
            }
            else {
                Console.WriteLine("There are {0} total records");
            }

            Console.WriteLine("");
            this.pitches = pitchesList;   
            this.hits = hitsList;      
        }

        /// <summary>
        /// Method <c>pitch</c> handles the actions and responses for when the user is pitching
        /// </summary>
        void pitch() {
            Console.WriteLine("Select Pitch Type: \nFast Ball (F) \nCurveball (C) \nKnuckle Ball (K)");

            string pitchType = Console.ReadLine();
            Random rnd = new Random();
            int rng = rnd.Next(1, 100);

            string pitchConversion = pitchType.ToLower() == "f" ? "fastBall" : pitchType.ToLower() == "c" ? "curveball" : "knuckleBall";
            PitchHit selectedPitch = this.pitches[this.pitches.FindIndex(p => p.name == pitchConversion)];
            
            if(rng < selectedPitch.strike) {
                this.advanceStrikes();
                this.printStats("STRIKE");
            }
            else if(rng < selectedPitch.ball) {
                this.advanceBalls();
                this.printStats("BALL");
            }
            else if(rng < selectedPitch.fly) {
                this.advanceOuts();
                this.printStats("Fly ball");
            }
            else if(rng < selectedPitch.first) {
                this.advanceBases(1);
                this.printStats("Hitter advances to FIRST");
            }
            else if(rng < selectedPitch.second) {
                this.advanceBases(2); 
                this.printStats("Hitter advances to SECOND");
            }
            else if(rng < selectedPitch.third) {
                this.advanceBases(3);
                this.printStats("Hitter advances to THIRD");
            }
            else {
                this.advanceBases(4);
                this.printStats("HOMERUN");
            }

        }

        /// <summary>
        /// Method <c>hit</c> handles the actions and responses for when the user is hitting
        /// </summary>
        void hit() {
            Console.WriteLine("Select Hit Type: \nPower (P) \nContact (C) \nBunt (B)");

            string hitType = Console.ReadLine();
            Random rnd = new Random();
            int rng = rnd.Next(1, 100);

            string hitConversion = hitType.ToLower() == "p" ? "power" : hitType.ToLower() == "c" ? "contact" : "bunt";
            PitchHit selectedHit = this.hits[this.hits.FindIndex(h => h.name == hitConversion)];
            
            if(rng < selectedHit.strike) {
                this.advanceStrikes();
                this.printStats("STRIKE");
            }
            else if(rng < selectedHit.ball) {
                this.advanceBalls();
                this.printStats("BALL");
            }
            else if(rng < selectedHit.fly) {
                this.advanceOuts();
                if(selectedHit.name == "bunt") this.printStats("Thrown OUT at home");
                else this.printStats("Fly ball -- OUT");
            }
            else if(rng < selectedHit.first) {
                this.advanceBases(1);
                this.printStats("Hitter advances to FIRST");
            }
            else if(rng < selectedHit.second) {
                this.advanceBases(2); 
                this.printStats("Hitter advances to SECOND");
            }
            else if(rng < selectedHit.third) {
                this.advanceBases(3);
                this.printStats("Hitter advances to THIRD");
            }
            else {
                this.advanceBases(4);
                this.printStats("HOMERUN");
            }
        }

        /// <summary>
        /// Method <c>advanceBases</c> handles adjusting class variables when a hit occurs, with the parameter <c>hit</c> indicating the number of bases the hitter reached
        /// </summary>
        /// <param name="hit"></param>
        void advanceBases(int hit) {
            this.balls = this.strikes = 0;

            if(hit == 4) {
                List<bool> bases = [this.first, this.second, this.third];
                if(this.pitching == false) this.user += bases.Count(t => t) + 1;
                else this.comp += bases.Count(t => t) + 1;
                
                this.first = this.second = this.third = false;
            }
            else if(hit == 3) {
                List<bool> bases = [this.first, this.second, this.third];
                if(this.pitching == false) this.user += bases.Count(t => t);
                else this.comp += bases.Count(t => t);
                
                this.third = true;
                this.first = this.second = false;
            }
            else if(hit == 2) {
                List<bool> bases = [this.second, this.third];
                if(this.pitching == false) this.user += bases.Count(t => t);
                else this.comp += bases.Count(t => t);
                
                if(this.first == true) this.third = true;
                else this.third = false;

                this.second = true;
                this.first = false;
            }
            else {
                List<bool> bases = [this.third];
                if(this.pitching == false) this.user += bases.Count(t => t);
                else this.comp += bases.Count(t => t);

                if(this.second == true) this.third = true;
                else this.third = false;

                if(this.first == true) this.second = true;
                else this.second = false;

                this.first = true;
            }
        }

        /// <summary>
        /// Method <c>advanceInning<c/> handles transitioning to the next HALF inning 
        /// </summary>
        void advanceInning() {
            Console.WriteLine("Switching Sides");

            this.inning++;
            this.pitching = !this.pitching;

            this.first = this.second = this.third = false;
            this.strikes = this.balls = this.outs = 0;
        }

        /// <summary>
        /// Method <c>advanceOuts</c> handles increasing the out count and if needed, triggers <c>advanceInning</c>
        /// </summary>
        void advanceOuts() {
            Console.WriteLine("OUT");

            this.outs++;
            if(this.outs == 3) this.advanceInning();
            else this.strikes = this.balls = 0;
        }

        /// <summary>
        /// Method <c>advanceStrikes</c> handles increasing the strike count and if needed, triggers <c>advanceOuts</c>
        /// </summary>
        void advanceStrikes() {
            this.strikes++;
            if(this.strikes == 3) this.advanceOuts();
        }

        /// <summary>
        /// Method <c>advanceBalls</c> handles increasing the ball count and if needed, triggers <c>advanceBases</c>
        /// </summary>
        void advanceBalls() {
            this.balls++;
            if(this.balls == 4) this.advanceBases(1);
        }

        /// <summary>
        /// Method <c>printStats</c> handles printing the current statisitcs for the game
        /// </summary>
        void printStats(string result) {
            string tb_inning = this.inning % 2 == 0 ? "Top " + (this.inning % 2 + 1): "Bottom " + (this.inning % 2 + 1);
            string firstBase = "O", secondBase = "O", thirdBase = "O";
            if(this.first) firstBase = "X";
            if(this.second) secondBase = "X";
            if(this.third) thirdBase = "X";

            string tabs = "";
            for(int i = 0; i < 32 - result.Length; i++) tabs += " ";
            

            Console.WriteLine($"\n\t\t\t\t\tUser: {this.user} | Comp: {this.comp}");
            Console.WriteLine($"{result}{tabs}{tb_inning} | Outs: {this.outs} | Balls: {this.balls} | Strikes: {this.strikes}");
            Console.WriteLine($"\t\t\t\t\t    |   {secondBase}   |");
            Console.WriteLine($"\t\t\t\t\t    | {thirdBase}   {firstBase} |");
        }

        /// <summary>
        /// Method <c>decideHome</c> asks user to determine who is the home team
        /// </summary>
        void decideHome() {
            string home = "neither";

            while(home.ToLower() != "home" && home.ToLower() != "away") {
                Console.WriteLine("Would you like to be home or away?");
                home = Console.ReadLine();
            }

            if(home.ToLower() == "home") pitching = false;
        }

        /// <summary>
        /// Main function for the game
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            BaseballGame game = new BaseballGame();
            game.getPitches_Hits();
            game.decideHome();

            while(game.inning != 18) {
                if(game.pitching) game.pitch();
                else game.hit();

                Console.Write("PRESS ENTER");
                Console.ReadLine();
            }

            Console.WriteLine($"GAME OVER -- Final Score: User {game.user} | Comp {game.comp}");
        }

    }
}
