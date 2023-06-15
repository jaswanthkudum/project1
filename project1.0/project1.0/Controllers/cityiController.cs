using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_lastest.mode;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace apiweb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cityiController : ControllerBase
    {


        private readonly IConfiguration _configuration;
        public cityiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("cityidetails")]
        //[Route("countrydetails")]
        public List<cityi> cityidetails()
        {
            List<cityi> citylist = new List<cityi>();
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand("SELECT ciid,ciname,cidisplayname,cidiscription,cicountryid,cistateid,sname,cname FROM countritable INNER JOIN statetable ON  countritable.cid=statetable.scountryid INNER JOIN citytable ON statetable.stid = citytable.cistateid ", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cityi cou = new cityi();
                    cou.ciid = Int32.Parse(dt.Rows[i]["ciid"].ToString());
                    cou.ciname = dt.Rows[i]["ciname"].ToString();
                    cou.cname = dt.Rows[i]["cname"].ToString();
                    cou.sname = dt.Rows[i]["sname"].ToString();

                    cou.cidisplayname = dt.Rows[i]["cidisplayname"].ToString();
                    cou.cidiscription = dt.Rows[i]["cidiscription"].ToString();
                    cou.cicountryid = Int32.Parse(dt.Rows[i]["cicountryid"].ToString());
                    cou.cistateid = Int32.Parse(dt.Rows[i]["cistateid"].ToString());
                    citylist.Add(cou);

                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return citylist;
            //return loadList();
            // return listdb();
        }


        [HttpGet("{ciid}")]
        //[Route("getname")]
        public cityi getname(int ciid)
        {
            try
            {
                if (ciid <= 0)
                {
                    throw new Exception("please provide city id");
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return cityidetails().Where(e => e.ciid == ciid).FirstOrDefault();
        }

        [HttpPost("cityadd")]

        //[Route("countryadd")]
        public string cityadd(cityi Obj)

        {
            try
            {
                if (string.IsNullOrEmpty(Obj.ciname))
                {
                    throw new Exception("please provide the city name");
                }
                else if (Obj.cicountryid <= 0)
                {
                    throw new Exception("please provide valid country id");
                }
                else if (Obj.cistateid <= 0)
                {
                    throw new Exception("please provide valid state id");
                }
                else if (!string.IsNullOrEmpty(Obj.ciname))
                {
                    bool t2 = returntype(Obj.ciname, Obj.cicountryid, Obj.cistateid, Obj.ciid);
                    if (t2)
                    {
                        throw new Exception("entered name already exists");
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                        SqlCommand cmd = new SqlCommand("INSERT INTO citytable (ciname,cidisplayname,cidiscription,cicountryid,cistateid) VALUES ('" + Obj.ciname + "','" + Obj.cidisplayname + "','" + Obj.cidiscription + "','" + Obj.cicountryid + "','" + Obj.cistateid + "')", con);
                        // using(SqlCommand cmd = new SqlCommand($"insert into employeetable values({Obj.employeeid},{Obj.employeename},{Obj.employeesalary)", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }

                //List<employee> emplist = new List<employee>();
                //emplist.Add(Obj);
            }
            catch (Exception e)
            {
                throw (e);
            }

            return "sucessfully added";

        }
        [HttpDelete("deletecity")]
        //[Route("delete")]
        public string deletecity(string ciid)
        {
            try
            {
                if (string.IsNullOrEmpty(ciid))
                {
                    throw new Exception("please select atleast one row");
                }
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand($"delete from citytable where ciid in({ciid})", con);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                throw (e);
            }
            return "delete sucess";

        }

        [HttpPut("updatecity")]
        // [Route("update")]
        public string updatecity(cityi c)

        {

            try
            {


                if (string.IsNullOrEmpty(c.ciname))
                {
                    throw new Exception("please provide the state name");
                }
                else if (c.cicountryid <= 0)
                {
                    throw new Exception("please provide valid country id");
                }
                else if (c.cistateid <= 0)
                {
                    throw new Exception("please provide valid state id");
                }
                else if (c.ciid <= 0)
                {
                    throw new Exception("please provide valid city id");
                }

                else if (!string.IsNullOrEmpty(c.ciname))
                {

                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                    var query = @"UPDATE citytable SET ciname = '" + c.ciname + @"',cidisplayname='" + c.cidisplayname + @"',cidiscription='" + c.cidiscription + @"',cicountryid='" + c.cicountryid + @"',cistateid='" + c.cistateid + @"'  where ciid = '" + c.ciid + @"' ";
                    // DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    // SqlDataAdapter da = new SqlDataAdapter(cmd);





                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return "updated sucessfully";
        }

        [HttpGet("checking_duplicatecity")]
        public Boolean boole(string ci_name, int ci_countryid, int ci_id, int ci_stateid)
        {

            if (!string.IsNullOrEmpty(ci_name))
            {

                bool t2 = returntype(ci_name, ci_countryid, ci_id, ci_stateid);
                if (t2)
                {
                    return true;
                }
            }
            return false;

        }


        private bool returntype(string ci_name, int ci_countryid, int ci_id, int ci_stateid)
        {
            var que = $"select ciname,ciid,cicountryid,cistateid from citytable where ciname= '{ci_name}' and cicountryid ={ci_countryid} and cistateid={ci_stateid}";
            if (ci_id > 0)
            {
                que += $"and (ciid <>{ci_id}) ";
            }
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("default"));

            SqlCommand cmdo = new SqlCommand(que, conn);

            conn.Open();
            using (SqlDataReader reader = cmdo.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    return true;
                }
                reader.Close();

                conn.Close();
                return false;
            }

        }

    }
}





