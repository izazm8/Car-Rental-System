using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarDoor.Models;


namespace CarDoor.Controllers
{
    public class HomeController : Controller
    {

        DataClasses1DataContext dc = new DataClasses1DataContext();
        public ActionResult index()
        {
            addLocAndCards();
            return View();
        }

        public ActionResult Add()
        {
            string fname = Request["fname"];
            string lname = Request["lname"];
            string email = Request["email_"];
            string password = Request["password_"];
            string username = Request["uname"];


            if (fname == "" || lname == "" || email == "" || password == "" || username == "")
            {
                ViewBag.fill_all_info = "Please Fill all the Fields !";
                return View("register");
            }


            User user = new User
            {
                email = email,
                password = password,
                last_name = lname,
                first_name = fname,
                username = username,
                type = "user"
            };

            if (dc.Users.Any(u => u.email == email))
            {
                ViewBag.user_not_exists = "User Already Exists ";
                return View("register");
            }
            else
            {
                dc.Users.InsertOnSubmit(user);
                dc.SubmitChanges();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Login_()
        {
            string email = Request["email_"];
            string password = Request["password_"];

            if (email == "" || password == "")
            {
                ViewBag.fill_all_info = "Fill All Fields !";
                return View("Login");
            }

            var user = dc.Users.FirstOrDefault(u => u.email == email && u.password == password);
            if (user != null)
            {
                Session["email"] = user.email;
                Session["fname"] = user.first_name;
                Session["user_id"] = user.Id;
                //Session["type"] = user.type;
                ViewBag.session_email = user.email;

                if (user.type == "admin")
                {
                    Session["type"] = "admin";
                    ViewBag.pagename = "adminpanel";
                    return RedirectToAction("adminpanel");
                }
                Session["type"] = "user";
                addLocAndCards();
                return View("index");
            }
            else
            {
                ViewBag.user_not_correct = "User Not Found !";
                return View("Login");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult article()
        {
            return View();
        }
        public ActionResult article_details()
        {
            return View();
        }

        public ActionResult car_details()
        {
            return View();
        }

        public ActionResult car_left_sidebar()
        {
            return View();
        }

        public ActionResult car_right_sidebar()
        {
            addCarstoViewBags();
            return View();
        }

        public ActionResult car_without_sidebar()
        {
            return View();
        }

        public ActionResult driver()
        {
            addDrivers();
            return View();
        }
        public ActionResult faq()
        {
            return View();
        }

        public ActionResult gallery()
        {
            return View();
        }

        public ActionResult help_desk()
        {
            return View();
        }

        public ActionResult login()
        {
            return View();
        }
        public ActionResult package()
        {
            return View();
        }

        public ActionResult register()
        {
            ViewBag.user_not_exists = "";
            return View();
        }

        public ActionResult services()
        {
            return View();
        }
        public ActionResult _404()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("index");
        }

        public ActionResult Book()
        {

            if (Session["email"] != null)
            {

                string pick_up_location = Request["pick_up_location"];
                string pick_up_date = Request["pick_up_date"];
                string return_date = Request["return_date"];
                string choose_car = Request["choose_car"];
                string choose_driver = Request["choose_driver"];

                if (pick_up_date == "" || return_date == "" || choose_car == "" || pick_up_location == "" || choose_driver == "")
                {
                    return View("index");
                }

                double days = (DateTime.Parse(return_date) - DateTime.Parse(pick_up_date)).Days;
                if (days <= 0)
                {
                    return View("correct_date");
                }

                booking booking = new booking();
                booking.pick_up_location = pick_up_location;
                booking.pick_up_date = pick_up_date;
                booking.car = choose_car;
                booking.return_date = return_date;
                booking.user_email = Session["email"].ToString();
                booking.user_id = Session["user_id"].ToString();
                booking.iscompleted = "no";

                double day = (Convert.ToDateTime(return_date) - Convert.ToDateTime(pick_up_date)).TotalDays;
                //int day = dt.Days;
                int driver_factor = 0;
                if (choose_driver != "" || choose_driver != "Select")
                {
                    booking.driver = choose_driver;
                    driver_factor = 20;
                }
                booking.bill_with_fine = (day * (35 + driver_factor)).ToString();
                booking.bill_wo_fine = booking.bill_with_fine;

                dc.bookings.InsertOnSubmit(booking);
                dc.SubmitChanges();


                return View("index");
            }
            ViewBag.please_login = "Please Login !";
            return View("login");
        }

        public ActionResult adminpanel()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if (Session["type"].ToString() == "admin")
            {
                ViewBag.pagename = "adminpanel";
                return View();
            }

            return View("login");
        }

        public ActionResult add_driver()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            if (Session["type"].ToString() == "admin")
            {
                ViewBag.pagename = "adminpanel";
                return View();
            }
            return View("login");
        }

        public ActionResult add_cars()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            if (Session["type"].ToString() == "admin")
            {
                ViewBag.pagename = "adminpanel";
                return View();
            }
            return View("login");
        }

        public ActionResult add_cars_(HttpPostedFileBase file)
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if (Session["type"].ToString() == "admin" && file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/img/car"), pic);
                file.SaveAs(path);

                string company_name = Request["company_name"];
                string car_model = Request["car_model"];
                string car_num = Request["car_num"];
                string car_rent = Request["car_rent"];
                string description = Request["description"];

                Car car = new Car();
                car.company = company_name;
                car.model = car_model;
                car.car_number = car_num;
                car.rent = car_rent;
                car.path = "~/Content/img/car/" + pic;
                car.description = description;
                car.rating = "2";

                dc.Cars.InsertOnSubmit(car);
                dc.SubmitChanges();

                ViewBag.pagename = "adminpanel";
                return View("add_cars");
            }
            return View("_404");
        }

        public ActionResult add_driver_(HttpPostedFileBase file)
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if ((Session["type"] == null || Session["type"].ToString() == "admin") && file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/img/driver"), pic);
                file.SaveAs(path);

                string driver_name = Request["driver_name"];
                string driver_address = Request["driver_address"];
                string driver_email = Request["driver_email"];
                string driver_fb_profile = Request["driver_fb_profile"];
                string driver_twitter_profile = Request["driver_twitter_profile"];
                string description = Request["description"];

                Driver driver = new Driver();
                driver.driver_name = driver_name;
                driver.address = driver_address;
                driver.facebook = driver_fb_profile;
                driver.twitter = driver_twitter_profile;
                driver.picture_path = "~/Content/img/driver/" + pic;

                driver.description = description;
                dc.Drivers.InsertOnSubmit(driver);
                dc.SubmitChanges();

                ViewBag.pagename = "adminpanel";
                return View("add_driver");
            }
            return View("_404");
        }

        public ActionResult edit_driver_(HttpPostedFileBase file)
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            if (Session["type"].ToString() == "admin" && file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/img/driver"), pic);
                file.SaveAs(path);

                string driver_name = Request["driver_name"];
                string driver_address = Request["driver_address"];
                string driver_email = Request["driver_email"];
                string driver_fb_profile = Request["driver_fb_profile"];
                string driver_twitter_profile = Request["driver_twitter_profile"];
                string description = Request["description"];


                var driver = (from d in dc.Drivers where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                driver.driver_name = driver_name;
                driver.address = driver_address;
                driver.facebook = driver_fb_profile;
                driver.mail = driver_email;
                driver.twitter = driver_twitter_profile;
                driver.picture_path = "~/Content/img/driver/" + pic; //System.IO.Path.Combine(
                //Server.MapPath("~/Content/img/driver"), pic);
                driver.description = description;

                //Driver driver = new Driver();
                //driver.driver_name = driver_name;
                //driver.address = driver_address;
                //driver.facebook = driver_fb_profile;
                //driver.twitter = driver_twitter_profile;
                //driver.picture_path = "~/Content/img/driver/" + pic; //System.IO.Path.Combine(
                //Server.MapPath("~/Content/img/driver"), pic);

                //dc.Drivers.InsertOnSubmit(driver);
                dc.SubmitChanges();

                ViewBag.pagename = "adminpanel";
                addDrivers();
                return View("modify_drivers");
            }
            return View("_404");
        }

        public ActionResult edit_cars_(HttpPostedFileBase file)
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            if (Session["type"].ToString() == "admin"&& file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/img/car"), pic);
                file.SaveAs(path);

                string company_name = Request["company_name"];
                string car_model = Request["car_model"];
                string car_num = Request["car_num"];
                string car_rent = Request["car_rent"];
                string description = Request["description"];


                var car = (from d in dc.Cars where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                car.company = company_name;
                car.model = car_model;
                car.car_number = car_num;
                car.rent = car_rent;
                car.description = description;
                car.path = "~/Content/img/car/" + pic; //System.IO.Path.Combine(
                                                       //Server.MapPath("~/Content/img/driver"), pic);


                //Driver driver = new Driver();
                //driver.driver_name = driver_name;
                //driver.address = driver_address;
                //driver.facebook = driver_fb_profile;
                //driver.twitter = driver_twitter_profile;
                //driver.picture_path = "~/Content/img/driver/" + pic; //System.IO.Path.Combine(
                //Server.MapPath("~/Content/img/driver"), pic);

                //dc.Drivers.InsertOnSubmit(driver);
                dc.SubmitChanges();

                ViewBag.pagename = "adminpanel";
                ViewBag.cars = dc.Cars.ToList();
                return View("modify_cars");
            }
            return View("_404");
        }

        public ActionResult modify_drivers()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }


            addDrivers();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult modify_cars()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            ViewBag.cars = dc.Cars.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult edit_drivers()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            var driver = (from d in dc.Drivers where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
            ViewBag.driver_ = driver;
            ViewBag.pagename = "adminpanel";
            return View();
        }



        public ActionResult edit_cars()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }


            var car = (from c in dc.Cars where c.Id == Int32.Parse(Request["id"]) select c).SingleOrDefault();
            ViewBag.car_ = car;
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult edit_branches()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            var driver = (from d in dc.locations where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
            ViewBag.driver_ = driver;
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult edit_branches_()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            string branch_name = Request["branch_name"];
            var driver = (from d in dc.locations where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
            driver.stop_name = branch_name;
            dc.SubmitChanges();
            ViewBag.pagename = "adminpanel";
            ViewBag.all_branches = dc.locations.ToList();
            return View("all_branches");
        }

        public ActionResult ongoingbookings()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            ViewBag.allbookings = dc.bookings.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult disabled_branches()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            ViewBag.all_branches = dc.locations.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult disable_car()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if (Request["id"] != null)
            {
                var driver = (from d in dc.Cars where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                driver.status = "disabled";
                dc.SubmitChanges();
            }
            ViewBag.pagename = "adminpanel";
            ViewBag.cars = dc.Cars.ToList();
            return View("modify_cars");
        }

        public ActionResult disable_drivers()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if (Request["id"] != null)
            {
                var driver = (from d in dc.Drivers where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                driver.status = "disabled";
                dc.SubmitChanges();
            }
            addDrivers();
            ViewBag.pagename = "adminpanel";
            return RedirectToAction("modify_drivers");
        }

        public ActionResult checkout()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            if (Request["id"] != null)
            {
                var booking = (from d in dc.bookings where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                booking.iscompleted = "yes";

                DateTime dt = DateTime.Today;
                double days = (dt - DateTime.Parse(booking.return_date)).Days;
                if (days > 0)
                {
                    booking.fine = "200";
                    booking.bill_with_fine = (Int32.Parse(booking.bill_wo_fine) + 200).ToString();
                }

                dc.SubmitChanges();
            }
            ViewBag.allbookings = dc.bookings.ToList();
            ViewBag.pagename = "adminpanel";
            return View("ongoingbookings");
        }

        public ActionResult allbookings()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            ViewBag.allbookings = dc.bookings.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult add_branch()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult add_branch_()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            string branch_name = Request["branck_name"];
            location location = new location();
            location.stop_name = branch_name;

            dc.locations.InsertOnSubmit(location);
            dc.SubmitChanges();
            ViewBag.pagename = "adminpanel";
            return View("add_branch");

        }

        public ActionResult all_branches()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }

            ViewBag.pagename = "adminpanel";
            ViewBag.all_branches = dc.locations.ToList();
            return View();
        }

        public ActionResult completebookings()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            ViewBag.allbookings = dc.bookings.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult disabled_drivers()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            addDrivers();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public ActionResult enable_drivers()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            var driver = (from d in dc.Drivers where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
            driver.status = "yes";
            dc.SubmitChanges();
            addDrivers();
            ViewBag.pagename = "adminpanel";
            return RedirectToAction("disabled_drivers");
        }

        public ActionResult enable_cars()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            var car = (from c in dc.Cars where c.Id == Int32.Parse(Request["id"]) select c).SingleOrDefault();
            car.status = "yes";
            dc.SubmitChanges();
            ViewBag.cars = dc.Cars.ToList();
            ViewBag.pagename = "adminpanel";
            return View("disabled_cars");

        }

        public ActionResult success_booking()
        {
            if (Session["type"] == null)
                return View("login");

            

            return View();
        }

        public ActionResult disabled_cars()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            ViewBag.cars = dc.Cars.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }



        public ActionResult enable_branch()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            var branch = (from b in dc.locations where b.Id == Int32.Parse(Request["id"]) select b).SingleOrDefault();
            branch.status = "yes";
            dc.SubmitChanges();

            ViewBag.all_branches = dc.locations.ToList();
            ViewBag.pagename = "adminpanel";
            return View("disabled_branches");
        }

        public ActionResult booking()
        {
            if (Session["type"] == null)
                return View("login");

            

            ViewBag.allbookings = dc.bookings.ToList();
            return View();
        }

        public ActionResult disable_branch()
        {
            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            if (Request["id"] != null)
            {
                var driver = (from d in dc.locations where d.Id == Int32.Parse(Request["id"]) select d).SingleOrDefault();
                driver.status = "disabled";
                dc.SubmitChanges();
            }
            ViewBag.all_branches = dc.locations.ToList();
            ViewBag.pagename = "adminpanel";
            return View("all_branches");
        }

        public ActionResult show_all_branches()
        {

            if (Session["type"] == null)
                return View("login");

            if (Session["type"].ToString() == "user")
            {
                addLocAndCards();
                return View("index");
            }
            ViewBag.all_branches = dc.locations.ToList();
            ViewBag.pagename = "adminpanel";
            return View();
        }

        public void addLocAndCards()
        {
            ViewBag.cars = dc.Cars.ToList();
            ViewBag.locations = dc.locations.ToList();
            ViewBag.drivers = dc.Drivers.ToList();
        }

        public void addCarstoViewBags()
        {
            ViewBag.cars_info = dc.Cars.ToList();
        }
        public void addDrivers()
        {
            ViewBag.drivers_info = dc.Drivers.ToList();
            //Driver d = ViewBag.drivers_info[0];

        }
    }
}