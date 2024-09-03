using UnityEngine;

public class Constants 
{
    //This script used for internal constants only for minigames constants write it in TMKOCPlaySchoolConstants

    #region Common string
    public static string iAmLoggedIn = "iAmLoggedIn";
    #endregion

    #region Scene Names

    //public static string PlayschoolMainScene = "PlayschoolMainScene";
    //public static string TMKOCPlayBeachScene = "TMKOCPlayBeachScene";
    public static string SettingPanel = "SettingPanel";
    public static string SplashScreen = "splashScreenNe";
    #endregion

    #region Validation Messages
    public static string mobileNumberMissing = "Please Enter Mobile Number";
    public static string OTPMissing = "Please Enter OTP Number";
    public static string OTPCorrectMsg = "Please Enter correct Otp, try again !";
    public static string registrationMsg = "Please Enter missing details";
    public static string emailValidationMsg = "Please Enter Correct Email Address";
    public static string selectLang = "Please Select Lang";
    #endregion

    #region Registration Data
    //LoginData
    public static string ParentName = "parentName";
    public static string ChildName = "childName";
    public static string MobileNumber = "mobileNumber";
    public static string DateOfBirth = "dateOfBirth";
    public static string EmailId = "emailId";
    public static string Country = "Country";
    public static string State = "State";
    public static string City = "City";

    //Language
    public static string SelectedLang = "SelectedLang";
    //public static string LangName = "LangName";
    public static string LangId = "LangId";

    //Gender
    public static string Gender = "gender";

    //AgeData
    public static string SaveAge = "saveAge";
    public static string AgeId = "ageId";

    //ActivitiesData
    public static string saveActivity = "saveActivity";
    public static string ActivityId = "activityId";

    //subscriptionId
    public static string SubscriptionId = "subscriptionId";


    public static string currentStudentPlaying = "currentStudent";




    #endregion

    #region API's

    //Register API
    // public static string OldRegisterAPI = "http://13.200.173.193/api/Auth/user/register";
    public static string RegisterAPI = "https://api-playschool.tmkocplayschool.com/api/Auth/user/register";

    public static string AddStudentAPI = "https://api-playschool.tmkocplayschool.com/api/Students/user/addstudent";

    //GetAll Language
    public static string GetAllLanguagesFromServer = "http://13.200.173.193/api/Languages/user/getalllanguage";

    //GetAgeListFromServer
    public static string GetAllAgeDetailsFromServer = "http://13.200.173.193/api/AgeGroup/user/getallagegroups";

    //GetActivitiesListFromServer
    public static string GetAllActivitiesFromServer = "http://13.200.173.193/api/ActivityPreferences/admin/getallactivitypreferences";
    
    //Get All Subscription Plans
    public static string GetAllSubscriptionPlans = "http://13.200.173.193/api/SubscriptionPlan/user/getsubscriptionplans";

    //Student Form Data
    public static string StudentFormData = "http://13.200.173.193/api/Students/user/studentinfoform";

    //is user is already Register
  //  public static string CheckUserIsAlreadyRegisteredOrNot = "http://13.200.173.193/api/Auth/user/isregistereduser";
    public static string CheckUserIsAlreadyRegisteredOrNot = "https://api-playschool.tmkocplayschool.com/api/Auth/user/isregistereduser";

    //check authtoken if user already registered
    public static string loginUrl = "http://13.200.173.193/api/Auth/user/login";

   // check authtoken if user already have auth token
    public static string AlredyAuth = "https://api-playschool.tmkocplayschool.com/api/Auth/user/isLoggedIn";


    public static string paymentApi = "https://api-playschool.tmkocplayschool.com/api/Payment/paymentlink";

    public static string AddAttendanceUrl = "https://api-playschool.tmkocplayschool.com/api/Students/user/addstudentattendance";

    public static string GetAttendanceUrl = "https://api-playschool.tmkocplayschool.com/api/Students/user/getstudentattandance";

    public static string GetStudentsFromUser = "https://api-playschool.tmkocplayschool.com/api/Auth/user/GetUserStudents";


    public static string CheckStudentPlaying = "https://api-playschool.tmkocplayschool.com/api/Students/user/studentPlayingOn";

    


    //used in SendAndFetchStudentProgressAndTest script
    public static string testProgressUrl = "http://13.200.173.193/api/StudentTests/user/getstudenttestprogress";
    public static string gameProgressUrl = "http://13.200.173.193/api/StudentGames/user/getstudentgameprogress";
    public static string getStudentAchievementProgressUrl = "http://13.200.173.193/user/getstudentachievementprogress";
    public static string getAchievementByIdUrl = "http://13.200.173.193/api/Achievement/user/getachievementbyId";
    public static string getstudentallachievement = "http://13.200.173.193/user/getstudentallachievements";


    public static string GetDataStudentGameById = "https://api-playschool.tmkocplayschool.com/api/StudentGames/user/getstudentgamebyId";

    public static string AddDataStudentGameById = "https://api-playschool.tmkocplayschool.com/api/StudentGames/user/addstudentgame";

    public static string GetDataStudentTestById = "https://api-playschool.tmkocplayschool.com/api/StudentTests/user/getstudenttestbyId";

    public static string AddDataStudentTestById = "https://api-playschool.tmkocplayschool.com/api/StudentTests/user/addstudenttest";

    //Add is Test is Remaining

   public static string GetReportCardStudent = "https://api-playschool.tmkocplayschool.com/api/StudentCategory/user/report";
    #endregion

    #region GameLoader Script
    //public static string gameDataUrl = "http://13.200.173.193/api/Content/user/getcontentbyName";
    public static string gameDataUrl = "https://api-playschool.tmkocplayschool.com/api/Category/user/getcateorybyName";
    #endregion

    #region Country+state+City API's www.universal-tutorial.com/api/
    public static string getAccessTockenUrl = "https://www.universal-tutorial.com/api/getaccesstoken";
    public static string getCountries = "https://www.universal-tutorial.com/api/countries/";
    public static string getStates = "https://www.universal-tutorial.com/api/states/";
    public static string getCities = "https://www.universal-tutorial.com/api/cities/";
    #endregion
}
