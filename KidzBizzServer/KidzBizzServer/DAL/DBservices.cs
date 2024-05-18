﻿using System.Data.SqlClient;
using System.Data;
using KidzBizzServer.BL;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{

    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! USER !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method return all the App Users
    //--------------------------------------------------------------------------------------------------

    public List<User> ReadUsers()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<User> users = new List<User>();

        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetUsers");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            User u = new User();
            u.UserId = Convert.ToInt32(dataReader["UserId"]);
            u.Username = dataReader["Username"].ToString();
            u.Password = dataReader["Password"].ToString();
            u.FirstName = dataReader["FirstName"].ToString();
            u.LastName = dataReader["LastName"].ToString();
            u.AvatarPicture = dataReader["AvatarPicture"].ToString();
            u.DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"]);
            u.Gender = dataReader["Gender"].ToString();
            u.Score = Convert.ToInt32(dataReader["Score"]);

            users.Add(u);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return users;
    }

    private SqlCommand buildReadStoredProcedureCommand(SqlConnection con, string spName)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        return cmd;

    }

    //-------------------------------------------------------------------------------------------------
    // This method registered user 
    //-------------------------------------------------------------------------------------------------

    public int RegisterUser(User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateRegisterUserWithStoredProcedure("KBSP_InsertUser", con, user);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }
    //--------------------------------------------------------------------------------------------------
    // Create the insert user SqlCommand using a stored procedure
    //--------------------------------------------------------------------------------------------------
    private SqlCommand CreateRegisterUserWithStoredProcedure(String spName, SqlConnection con, User user)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Username", user.Username);

        cmd.Parameters.AddWithValue("@Password", user.Password);

        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);

        cmd.Parameters.AddWithValue("@LastName", user.LastName);

        cmd.Parameters.AddWithValue("@AvatarPicture", user.AvatarPicture);

        cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);

        cmd.Parameters.AddWithValue("@Gender", user.Gender);

        cmd.Parameters.AddWithValue("@Score", user.Score);


        return cmd;
    }

    //this method delete user

    public int DeleteUser(string username)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateDeleteUserCommandWithStoredProcedure("KBSP_DeleteUser", con, username);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }



    private SqlCommand CreateDeleteUserCommandWithStoredProcedure(String spName, SqlConnection con, string username)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Username", username);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // This method update user
    //-------------------------------------------------------------------------------------------------

    public User UpdateUser(User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateUpdateUserCommandWithStoredProcedure("KBSP_UserUpdate", con, user);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            if (numEffected == 1)
            {
                return user;
            }
            return null;
        }


        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // Create the update user SqlCommand using a stored procedure
    //--------------------------------------------------------------------------------------------------
    private SqlCommand CreateUpdateUserCommandWithStoredProcedure(String spName, SqlConnection con, User user)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Username", user.Username);

        cmd.Parameters.AddWithValue("@Password", user.Password);

        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);

        cmd.Parameters.AddWithValue("@LastName", user.LastName);

        cmd.Parameters.AddWithValue("@AvatarPicture", user.AvatarPicture);

        cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);

        cmd.Parameters.AddWithValue("@Gender", user.Gender);

        cmd.Parameters.AddWithValue("@Score", user.Score);


        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // This method login user
    //-------------------------------------------------------------------------------------------------

    public User LoginUser(string username, string password)
    {

        SqlConnection con;
        SqlCommand cmd;
        User authenticatedUser = null;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        cmd = buildReadStoredProcedureCommandLoginUser(con, "KBSP_UserLogin", username, password);

        try
        {


            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (dataReader.Read())
            {
                authenticatedUser = new User
                {
                    UserId = Convert.ToInt32(dataReader["UserId"]),
                    Username = dataReader["Username"].ToString(),
                    Password = dataReader["Password"].ToString(),
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    AvatarPicture = dataReader["AvatarPicture"].ToString(),
                    DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"]),
                    Gender = dataReader["Gender"].ToString(),
                    Score = Convert.ToInt32(dataReader["Score"])

                };

            }
            return authenticatedUser;
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }


    }

    private SqlCommand buildReadStoredProcedureCommandLoginUser(SqlConnection con, String spName, string username, string password)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Password", password);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // This method read user by username
    //-------------------------------------------------------------------------------------------------

    public User ReadUserByUsername(string username)
    {
        SqlConnection con;
        SqlCommand cmd;
        User user = null;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = buildReadStoredProcedureCommandReadUserByUsername(con, "KBSP_GetUserByUsername", username);

        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (dataReader.Read())
            {
                user = new User
                {
                    Username = dataReader["Username"].ToString(),
                    Password = dataReader["Password"].ToString(),
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    AvatarPicture = dataReader["AvatarPicture"].ToString(),
                    DateOfBirth = Convert.ToDateTime(dataReader["DateOfBirth"]),
                    Gender = dataReader["Gender"].ToString(),
                    Score = Convert.ToInt32(dataReader["Score"])
                };
            }
            return user;
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    private SqlCommand buildReadStoredProcedureCommandReadUserByUsername(SqlConnection con, string spName, string username)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@username", username);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! PLAYER !!!
    //-------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------
    // This method insert player  
    //-------------------------------------------------------------------------------------------------
    public int InsertPlayer(Player player)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // יצירת חיבור למסד נתונים
        }
        catch (Exception ex)
        {
            // כתיבה ללוג
            throw (ex);
        }

        cmd = CreateInsertPlayerCommand("KBSP_InsertPlayer", con, player); // יצירת פקודת SQL

        try
        {
            cmd.ExecuteNonQuery();
            // Get the PlayerId from the result
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    player.PlayerId = reader.GetInt32(0);
                }
            }

            return player.PlayerId;
        }
        catch (Exception ex)
        {
            // כתיבה ללוג
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // סגירת חיבור למסד נתונים
            }
        }
    }

    private SqlCommand CreateInsertPlayerCommand(String spName, SqlConnection con, Player player)
    {
        SqlCommand cmd = new SqlCommand(); // יצירת אובייקט פקודה

        cmd.Connection = con;            // הגדרת החיבור לפקודה
        cmd.CommandText = spName;        // שם ה-Stored Procedure
        cmd.CommandType = CommandType.StoredProcedure; // סוג הפקודה

        // הוספת פרמטרים לפקודה
        cmd.Parameters.AddWithValue("@UserId", player.User.UserId);
        cmd.Parameters.AddWithValue("@CurrentPosition", player.CurrentPosition);
        cmd.Parameters.AddWithValue("@CurrentBalance", player.CurrentBalance);
        cmd.Parameters.AddWithValue("@PlayerStatus", player.PlayerStatus);
        cmd.Parameters.AddWithValue("@LastDiceResult", player.LastDiceResult);

        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // This method return all the App Players
    //--------------------------------------------------------------------------------------------------

    public List<Player> ReadPlayers()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Player> players = new List<Player>();


        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetPlayers");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Player player = new Player();
            player.User = new User();
            player.PlayerId = Convert.ToInt32(dataReader["PlayerId"]);
            player.User.Username = dataReader["Username"].ToString();
            player.User.AvatarPicture = dataReader["AvatarPicture"].ToString();
            player.CurrentPosition = Convert.ToInt32(dataReader["CurrentPosition"]);
            player.CurrentBalance = Convert.ToDouble(dataReader["CurrentBalance"]);
            player.PlayerStatus = dataReader["PlayerStatus"].ToString();
            player.LastDiceResult = Convert.ToInt32(dataReader["LastDiceResult"]);
            player.Properties = ReadPropertiesByPlayerId(player.PlayerId);
            players.Add(player);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return players;
    }

    // this method AddPropertyToPlayer
    public int AddPropertyToPlayer(int playerId, int propertyId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateAddPropertyToPlayerCommand("KBSP_AddPropertyToPlayer", con, playerId, propertyId); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    private SqlCommand CreateAddPropertyToPlayerCommand(String spName, SqlConnection con, int playerId, int propertyId)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        cmd.Parameters.AddWithValue("@PropertyId", propertyId);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // This method update Player
    //-------------------------------------------------------------------------------------------------

    public Player UpdatePlayer(Player player)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // יצירת חיבור למסד נתונים
        }
        catch (Exception ex)
        {
            // כתיבה ללוג
            throw (ex);
        }

        cmd = CreateUpdatePlayerCommand("KBSP_PlayerUpdate", con, player); // יצירת פקודת SQL

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            if (numEffected == 1)
            {
                return player;
            }
            return null;
        }
        catch (Exception ex)
        {
            // כתיבה ללוג
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // סגירת חיבור למסד נתונים
            }
        }
    }

    private SqlCommand CreateUpdatePlayerCommand(String spName, SqlConnection con, Player player)
    {
        SqlCommand cmd = new SqlCommand(); // יצירת אובייקט פקודה

        cmd.Connection = con;            // הגדרת החיבור לפקודה
        cmd.CommandText = spName;        // שם ה-Stored Procedure
        cmd.CommandType = CommandType.StoredProcedure; // סוג הפקודה

        // הוספת פרמטרים לפקודה

        cmd.Parameters.AddWithValue("@CurrentPosition", player.CurrentPosition);
        cmd.Parameters.AddWithValue("@CurrentBalance", player.CurrentBalance);
        cmd.Parameters.AddWithValue("@PlayerStatus", player.PlayerStatus);
        cmd.Parameters.AddWithValue("@LastDiceResult", player.LastDiceResult);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! PROPERTY !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method return properties by id
    //--------------------------------------------------------------------------------------------------
    public List<Property> ReadPropertiesByPlayerId(int playerId)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Property> properties = new List<Property>();

        cmd = buildReadStoredProcedureCommandReadPropertiesByPlayer(con, "KBSP_GetPropertiesByPlayer", playerId);

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Property p = new Property();
            p.PropertyId = Convert.ToInt32(dataReader["PropertyId"]);
            p.PropertyName = dataReader["PropertyName"].ToString();
            p.PropertyPrice = Convert.ToDouble(dataReader["PropertyPrice"]);

            properties.Add(p);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return properties;
    }

    private SqlCommand buildReadStoredProcedureCommandReadPropertiesByPlayer(SqlConnection con, string spName, int playerId)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@PlayerId", playerId);

        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // This method return all the Properties
    //--------------------------------------------------------------------------------------------------

    public List<Property> ReadProperties()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Property> properties = new List<Property>();


        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetProperties");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Property p = new Property();
            p.PropertyId = Convert.ToInt32(dataReader["PropertyId"]);
            p.PropertyName = dataReader["PropertyName"].ToString();
            p.PropertyPrice = Convert.ToDouble(dataReader["PropertyPrice"]);

            properties.Add(p);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return properties;
    }


    //-------------------------------------------------------------------------------------------------
    // !!! QUESTION !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method return all the Questions
    //--------------------------------------------------------------------------------------------------

    public List<Question> ReadQuestions()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        List<Question> questions = new List<Question>();


        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetQuestions");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Question q = new Question();
            q.QuestionId = Convert.ToInt32(dataReader["QuestionId"]);
            q.QuestionText = dataReader["QuestionText"].ToString();

            questions.Add(q);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return questions;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! FEEDBACK !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method return all the Feedbacks
    //--------------------------------------------------------------------------------------------------

    public List<Feedback> ReadFeedback()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        List<Feedback> feedbacks = new List<Feedback>();


        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetFeedback");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Feedback f = new Feedback();
            f.FeedbackId = Convert.ToInt32(dataReader["FeedbackId"]);
            f.UserId = Convert.ToInt32(dataReader["UserId"]);
            f.FeedbackDescription = dataReader["FeedbackDescription"].ToString();
            f.Rating = Convert.ToInt32(dataReader["Rating"]);

            feedbacks.Add(f);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return feedbacks;
    }


    //--------------------------------------------------------------------------------------------------
    // This method insert feedback
    //--------------------------------------------------------------------------------------------------

    public int InsertFeedback(Feedback feedback)

    {
        SqlConnection con;
        SqlCommand cmd;

        try

        {
            con = connect("myProjDB"); // create the connection
        }

        catch (Exception ex)

        {
            throw (ex);
        }

        cmd = CreateInsertFeedbackWithStoredProcedure("KBSP_InsertFeedback", con, feedback);             // create the command

        try

        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }

        catch (Exception ex)

        {
            throw (ex);
        }

        finally
        {
            if (con != null)

            {
                con.Close();
            }
        }
    }

    private SqlCommand CreateInsertFeedbackWithStoredProcedure(string spName, SqlConnection con, Feedback feedback)

    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;          // can be Select, Insert, Update, Delete

        cmd.CommandTimeout = 10;           // Time to wait for the execution. The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text
        cmd.Parameters.AddWithValue("@UserId", feedback.UserId);
        cmd.Parameters.AddWithValue("@FeedbackDescription", feedback.FeedbackDescription);
        cmd.Parameters.AddWithValue("@Rating", feedback.Rating);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! ANSWER !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method insert Answer
    //--------------------------------------------------------------------------------------------------

    // לא בטוחה שצריך פונקציה כזאת של הכנסת תשובה מהקליינט 



    public int InsertAnswer(Answer answer)

    {

        SqlConnection con;
        SqlCommand cmd;

        try

        {
            con = connect("myProjDB"); // create the connection
        }

        catch (Exception ex)

        {
            // write to log
            throw (ex);

        }

        cmd = CreateInsertAnswerWithStoredProcedure("KBSP_InsertAnswer", con, answer);             // create the command

        try

        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }

        catch (Exception ex)

        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)

            {
                // close the db connection
                con.Close();
            }
        }

    }

    private SqlCommand CreateInsertAnswerWithStoredProcedure(String spName, SqlConnection con, Answer answer)

    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object
        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete
        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@QuestionID", answer.QuestionId);
        cmd.Parameters.AddWithValue("@AnswerText", answer.AnswerText);
        cmd.Parameters.AddWithValue("@IsCorrect", answer.IsCorrect);
        return cmd;

    }

    //-------------------------------------------------------------------------------------------------
    // !!! GAME !!!
    //-------------------------------------------------------------------------------------------------

    // this method insert game
    public int InsertGame(Game game)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateInsertGameWithStoredProcedure("KBSP_InsertGame", con, game);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    private SqlCommand CreateInsertGameWithStoredProcedure(String spName, SqlConnection con, Game game)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@NumberOfPlayers", game.NumberOfPlayers);
        cmd.Parameters.AddWithValue("@GameDuration", game.GameDuration); // Assuming seconds is a suitable unit

        cmd.Parameters.AddWithValue("@GameStatus", game.GameStatus);

        cmd.Parameters.AddWithValue("@GameTimestamp", game.GameTimestamp);

        return cmd;
    }

    // this method read game

    public List<Game> ReadGames()
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> games = new List<Game>();

        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetGames");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Game game = new Game();
            game.GameId = Convert.ToInt32(dataReader["GameId"]);
            game.NumberOfPlayers = Convert.ToInt32(dataReader["NumberOfPlayers"]);
            game.GameDuration = dataReader["GameDuration"].ToString();
            game.GameStatus = dataReader["GameStatus"].ToString();
            game.GameTimestamp = Convert.ToDateTime(dataReader["GameTimestamp"]);

            games.Add(game);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return games;
    }



    // this method update game
    public Game UpdateGame(Game game)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateUpdateGameCommandWithStoredProcedure("KBSP_GameUpdate", con, game); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            if (numEffected == 1)
            {
                return game;
            }
            return null;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    private SqlCommand CreateUpdateGameCommandWithStoredProcedure(String spName, SqlConnection con, Game game)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con; // assign the connection to the command object

        cmd.CommandText = spName; // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10; // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@GameId", game.GameId);
        cmd.Parameters.AddWithValue("@NumberOfPlayers", game.NumberOfPlayers);
        cmd.Parameters.AddWithValue("@GameDuration", game.GameDuration);
        cmd.Parameters.AddWithValue("@GameStatus", game.GameStatus);
        cmd.Parameters.AddWithValue("@GameTimestamp", game.GameTimestamp);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------
    // !!! Card !!!
    //-------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------
    // This method return all the App Cards
    //--------------------------------------------------------------------------------------------------

    public List<Card> ReadCards()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Card> cards = new List<Card>();

        cmd = buildReadCardStoredProcedureCommand(con, "KBSP_GetCards");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        ;
        while (dataReader.Read())
        {
            Card card = new Card();
            card.CardId = Convert.ToInt32(dataReader["CardID"]);
            card.Description = dataReader["Description"].ToString();
            card.Action = (CardAction)Convert.ToInt32(dataReader["ActionType"]);
            card.Amount = Convert.ToInt32(dataReader["Amount"]);
            card.MoveTo = Convert.ToInt32(dataReader["MoveTo"]);



            cards.Add(card);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return cards;
    }

    private SqlCommand buildReadCardStoredProcedureCommand(SqlConnection con, string spName)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        return cmd;

    }
    //-------------------------------------------------------------------------------------------------
    // This method updates a card
    //-------------------------------------------------------------------------------------------------

    public Card UpdateCard(Card card)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateUpdateCardCommandWithStoredProcedure("KBSP_CardUpdate", con, card); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            if (numEffected == 1)
            {
                return card;
            }
            return null;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }

    //--------------------------------------------------------------------------------------------------
    // Create the update card SqlCommand using a stored procedure
    //--------------------------------------------------------------------------------------------------
    private SqlCommand CreateUpdateCardCommandWithStoredProcedure(String spName, SqlConnection con, Card card)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con; // assign the connection to the command object

        cmd.CommandText = spName; // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10; // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@CardID", card.CardId);
        cmd.Parameters.AddWithValue("@Description", card.Description);
        cmd.Parameters.AddWithValue("@ActionType", card.Action);
        cmd.Parameters.AddWithValue("@Amount", card.Amount);
        cmd.Parameters.AddWithValue("@MoveTo", card.MoveTo);

        return cmd;
    }

    //-------------------------------------------------------------------------------------------------

    public List<Answer> ReadAnswers()
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Answer> answers = new List<Answer>();

        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetAnswers");

        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (dataReader.Read())
        {
            Answer a = new Answer();
            a.AnswerId = Convert.ToInt32(dataReader["AnswerId"]);
            a.QuestionId = Convert.ToInt32(dataReader["QuestionId"]);
            a.AnswerText = dataReader["AnswerText"].ToString();
            a.IsCorrect = Convert.ToBoolean(dataReader["IsCorrect"]);

            answers.Add(a);
        }
        if (con != null)
        {
            // close the db connection
            con.Close();
        }
        return answers;

    }

    public (decimal, int) GetGameSettings()    // from Game setting table 
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB");
        }
        catch (Exception ex)
        {
            throw (ex);
        }

        cmd = buildReadStoredProcedureCommand(con, "KBSP_GetGameSettings");
        SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        while (dataReader.Read())
        {
            decimal startingMoney = Convert.ToDecimal(dataReader["StartingMoney"]);
            int currentLocation = Convert.ToInt32(dataReader["CurrentLocation"]);
            return (startingMoney, currentLocation);

        }
        if (con != null)
        {
            con.Close();
        }
        return (0, 0);
    }


    public void UpdatePlayerPosition(int playerId, int newPosition , int lastDiceResult)
    {
        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateUpdatePosCommandWithStoredProcedure("KBSP_UpdatePlayerPosition", con, playerId, newPosition, lastDiceResult); // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery();
            if (numEffected != 1)
            {
                // Handle the case where the update was not successful
            }
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }

    private SqlCommand CreateUpdatePosCommandWithStoredProcedure(String spName, SqlConnection con, int playerId, int newPosition, int lastDiceResult)
    {
        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con; // assign the connection to the command object

        cmd.CommandText = spName; // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10; // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@playerId", playerId);
        cmd.Parameters.AddWithValue("@newPosition", newPosition);
        cmd.Parameters.AddWithValue("@lastDiceResult", lastDiceResult);


        return cmd;
    }




}













