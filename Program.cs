using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;


namespace ValidatePersonnummer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Har menyn öppen tills showMenu retunerar false
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }

        }

        public static int kontrollsumma_personnummer(string personnummer)
        {
            int value = 0;
            for (int i = 0; i < personnummer.Length; i++)
            {
                int t = (personnummer[i] - 48)          //Subtraherar personnummret med 48
                    << (1 - (i & 1));                  // Multiplicera med två varannan gång
                if (t > 9) t = t - 9;                  // Om talet är större än 9 ska man subtrahera 9 (Siffersumman = talet - 9).
                value += t;
            }
            return (value % 10);                       // Returnerar resten från kontrollsumman dividerat med 10 (ett gilltigt personnumret måste få resten 0).
        }

        static bool kontrollera_OrganisationsNummer(string orgnr)
        {
            //Om strängen är längre än eller lika med 12 tar vi bort de första 2 siffrorna för att få korrekt formattering
            if (orgnr.Length >= 12)
            {
                orgnr = orgnr.Remove(0, 2);
            }

            Regex rg = new Regex(@"^(\d{1})(\d{5})\-(\d{4})$");
            Match matches = rg.Match(orgnr);

            //Om orgnr är felformatterat är det ej giltigt
            if (!matches.Success)
                return false;

            //Delar upp organisationsnummret i olika grupper
            string group = matches.Groups[1].Value;
            string controlDigits = matches.Groups[3].Value;
            string allDigits = group + matches.Groups[2].Value + controlDigits;

            //Om man vill utveckla metoden kan man titta på första siffran "group" och utifrån det ange vilken typ av organistaion det är :)

            if (Int32.Parse(allDigits.Substring(2, 1)) < 2)
                return false;

            string nn = "";

            for (int n = 0; n < allDigits.Length; n++)
            {
                nn += ((((n + 1) % 2) + 1) * Int32.Parse(allDigits.Substring(n, 1)));
            }

            int checkSum = 0;

            for (int n = 0; n < nn.Length; n++)
            {
                checkSum += Int32.Parse(nn.Substring(n, 1));
            }

            return checkSum % 10 == 0 ? true : false; // Returnerar resten från kontrollsumman dividerat med 10 (ett gilltigt personnumret måste få resten 0).
        }

        public static string kontrollera_kön(string personnummer) //För att hitta om det är en man eller kvinna kollar man om könssiffran är jämnt delbart med två.
        {
            int a = personnummer[8] - 48;

            if ((a % 2) == 1) //Om näst sista siffran inte är jämnt delbart med 2 är det en Man.
            {
                return "Man";
            }
            else // Om det är jämnt delbart med 2 är det en Kvinna.
            {
                return "Kvinna";
            }
        }

        public static void kontrollera_personnummer(string personnummer)
        {
            var regex = new Regex(@"^(\d{2}){0,1}(\d{2})(\d{2})(\d{2})([-|+]{0,1})?(\d{3})(\d{0,1})$");
            Match matches = regex.Match(personnummer);
            
            //Om strängen är längre än eller lika med 12 tar vi bort de första 2 siffrorna för att få korrekt formattering
            if (personnummer.Length >= 12)
            {
                personnummer = personnummer.Remove(0, 2);
            }

            //Kollar så att formatteringen är korrekt
            if (matches.Success)
            {
                    //Tar bort + och - i personnummret för att få en korrekt input som jag kan räkna med
                    personnummer = personnummer.Replace("-", string.Empty);
                    personnummer = personnummer.Replace("+", string.Empty);
                    if (kontrollsumma_personnummer(personnummer) == 0)
                    {
                        Console.WriteLine("Personnumret är giltigt");
                    }
                
                else
                {
                    Console.WriteLine("Personnumret är ej giltigt");
                }
            }
            else
            {
                Console.WriteLine("Felaktig formatering på personnummret");
            }
        }

        public static void kontrollera_sammordningsnummer(string sammordningsnummer)
        {
            //Om strängen är längre än eller lika med 12 tar vi bort de första 2 siffrorna för att få korrekt formattering
            if (sammordningsnummer.Length >= 12)
            {
                sammordningsnummer = sammordningsnummer.Remove(0, 2);
            }

            var regex = new Regex(@"^(\d{2}){0,1}(\d{2})(\d{2})(\d{2})([-|+]{0,1})?(\d{3})(\d{0,1})$");
            Match matches = regex.Match(sammordningsnummer);

            //Hämtar alla värden från strängen
            string arString = matches.Groups[2].Value;
            string manadString = matches.Groups[3].Value;
            string datumString = matches.Groups[4].Value;
            string group = matches.Groups[6].Value;
            string sistaSiffran = matches.Groups[7].Value;

            //Kollar så att formatteringen är korrekt
            if (matches.Success)
            {
                //Gör om datument till en int för att ha möjlighet att subtrahera med 60
                int datumInt = Int16.Parse(datumString);
                int riktigtDatum = datumInt - 60;
                string riktigtDatumString = riktigtDatum.ToString();

                //Om man exemeplvis tagit 61-60=1 dvs personen är född första dagen på månaden måste man addera en nolla i början av strängen så man får 01
                if (riktigtDatumString.Length == 1)
                    riktigtDatumString = '0' + riktigtDatumString;

                //Konkatenerar min nya sträng med det riktigt datumet så jag kan validera det på samma sätt som personnummret
                string samordningsnr = arString + manadString + riktigtDatumString + group + sistaSiffran;
                //Tar bort + och - i personnummret för att få en korrekt input som jag kan räkna med
                samordningsnr = samordningsnr.Replace("-", string.Empty);
                samordningsnr = samordningsnr.Replace("+", string.Empty);
                //Om resten av kontrollsumman är 0 = giltigt
                if (kontrollsumma_personnummer(samordningsnr) == 0)
                {
                    Console.WriteLine("Samordningsnummret är giltigt");
                }
                //annars ej giltigt
                else
                {
                    Console.WriteLine("Samordningsnummret är ej giltigt");
                }
            }
            else
            {
                Console.WriteLine("Felaktig formatering på samordningsnummret");
            }
        }

        private static bool MainMenu()
        {
            //Meny
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Bekräfta Personnummer");
            Console.WriteLine("2) Bekräfta Samordningsnummer");
            Console.WriteLine("3) Bekräfta Organisationsnummer");
            Console.WriteLine("4) Exit");
            Console.Write("\r\nSelect an option: ");

            //Kör mina funktioner beroende på vilket siffra användaren väljer
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Skriv ett personnummer: ");
                    string personnummer = Console.ReadLine();
                    kontrollera_personnummer(personnummer);
                    return true;
                case "2":
                    Console.WriteLine("Skriv ett sammordningsnummer: ");
                    string samordningsnummer = Console.ReadLine();
                    kontrollera_sammordningsnummer(samordningsnummer);
                    return true;
                case "3":
                    Console.WriteLine("Skriv ett organisationsnummer: ");
                    string organisationsnummer = Console.ReadLine();
                    Console.WriteLine(kontrollera_OrganisationsNummer(organisationsnummer));
                    return true;
                //Retunerar falskt och loppen brtyds dvs programmet avslutas
                case "4":
                    return false;
                default:
                    return true;
            }
        }
    }
}