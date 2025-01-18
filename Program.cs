public class Query{
    public String? Name;
    public String? Location;
    public String? TemplateID;

    public Query(String Name ,String Location , String TemplateID){
        this.Name = Name;
        this.Location = Location;
        this.TemplateID = TemplateID;
    }
}

public class Utils{
    public static Query? query;
    public static Dictionary<String,String> TemplateIDs = new Dictionary<string, string>();

    public static void PrintError(Object o){
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(o);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void CopyFiles(String from, String to){
        foreach(String d in Directory.GetDirectories(from,"*",SearchOption.AllDirectories)){
            Directory.CreateDirectory(d.Replace(from,to));
        }
        foreach(String f in Directory.GetFiles(from,"*",SearchOption.AllDirectories)){
            File.Copy(f,f.Replace(from,to),true);
        }
    }
}

public class Program{

    public static void Main(String[] args){
        /////LOAD TEMPLATES
        try{
        String? processFolder = System.Environment.ProcessPath; //Get exe path of program
        processFolder = Path.GetDirectoryName(processFolder); //Get directory of exe

        processFolder = processFolder + "\\Templates";
        String[] dirs = Directory.GetDirectories(processFolder);
        
        foreach(String dir in dirs){
            Utils.TemplateIDs.Add(new DirectoryInfo(dir).Name,dir); //Add each subdir to TemplateID Dictionary
        }
        }catch{
            throw new Exception("Could not load templates. Make sure to create a folder named Templates in the .exe directory.");
        }

        /////ADD EACH COMMAND
        Dictionary<String,Func<String[],bool>> argList = new Dictionary<String,Func<String[],bool>>();
        argList.Add("--help",Help);
        argList.Add("--new",NewTemplate);
        argList.Add("--list",List);

        bool OK = true;
        //If no argument is provided, print recommendation.
        if(args.Length == 0) {Utils.PrintError("Try 'templateBuilder --help'"); System.Environment.Exit(0);}

        //Check each argument
        foreach(String s in args){
            if(argList.ContainsKey(s)){
                var result = argList[s].Invoke(args);
                if(result == false) OK=false;
            }
        }
        //If there was a correct '--new' argument, copy files
        if((Utils.query != null) && OK){
            try{
            String folderPath = Utils.query.Location + "\\" + Utils.query.Name;
            Directory.CreateDirectory(folderPath);
            Utils.CopyFiles(Utils.TemplateIDs[Utils.query.TemplateID],folderPath);
            }catch{
                throw new Exception("Could not copy files.");
            }
            Console.WriteLine("Project created! at: " + Utils.query.Location);
        }
    }

    /// <summary>
    /// Prints all commands.
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>True if there are no errors, False if there was an error</returns>
    public static bool Help(String[] args){
        if(args.Length > 1){
            Utils.PrintError("Too many arguments!");
            return false;
        }
        Console.WriteLine("--help - Prints a list of all available commands.");
        Console.WriteLine("--new [NAME] [TEMPLATEID]- Creates a new folder and copies a template at it's location.");
        Console.WriteLine("--list - Lists all template IDs.");
        return true;
    }

    /// <summary>
    /// Checks if provided arguments are correct and creates the Query object in Utils 
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>True if there are no errors, False if there was an error</returns>
    public static bool NewTemplate(String[] args){
        if(args.Length > 3){
            Utils.PrintError("Too many arguments!");
            return false;
        }
        

        //name index = current index + 1
        //templateID index = current index + 2
        int currentIndex = int.MinValue;
        int nameIndex = int.MinValue;
        int templateIDIndex = int.MinValue;

        for(int i = 0; i < args.Length; ++i){
            if(args[i] == "--new"){
                currentIndex = i;
                break;
            }
        }
        //If last index is bigger or the same as the templateID index.
        if((args.Length-1) >= (currentIndex+2)){
            nameIndex = currentIndex+1;
            templateIDIndex = currentIndex+2;
        }else{
            Utils.PrintError("Bad arguments! --new [NAME] [TEMPLATEID]");
            return false;
        }

        //Check if TemplateID exists
        if(!Utils.TemplateIDs.ContainsKey(args[templateIDIndex])){
            Utils.PrintError("There is no '" + args[templateIDIndex] + "' Template.");
            Utils.PrintError("Try --list for a list of all templates.");
            return false;
        }

        Utils.query = new Query(args[nameIndex],Directory.GetCurrentDirectory(),args[templateIDIndex]);
        return true;
    }

    /// <summary>
    /// Prints all TemplateIDs currently loaded.
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>True if there are no errors, False if there was an error</returns>
    public static bool List(String[] args){
        if(args.Length > 1){
            Utils.PrintError("Too many arguments!");
            return false;
        }

        var keys = Utils.TemplateIDs.Keys.ToArray<String>();
        if(keys.Length == 0){
            Console.WriteLine("No templates found. To create a template, simply add a new folder to the Templates folder next to the .exe file.");
        }
        
        for(int i = 0; i < keys.Length; ++i){
            Console.WriteLine(keys[i] + " --- " + Utils.TemplateIDs[keys[i]]);
        }
        return true;
    }
}
