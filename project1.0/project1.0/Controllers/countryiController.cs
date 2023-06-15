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
using System.Runtime.InteropServices;

namespace apiweb.Controllers


{




    [Route("api/[controller]")]
    [ApiController]
    public class countryiController : ControllerBase

    {
        private readonly IConfiguration _configuration;
        public countryiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }




        [HttpGet("countrydetails")]
        //[Route("countrydetails")]
        public List<countryi> countrydetails()
        {
            List<countryi> countrylist = new List<countryi>();
            try
            {

                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand("select * from countritable", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    countryi cou = new countryi();
                    cou.cid = (int)dt.Rows[i]["cid"];

                    cou.cname = dt.Rows[i]["cname"].ToString();
                    cou.cdisplayname = dt.Rows[i]["cdisplayname"].ToString();
                    cou.cdiscription = dt.Rows[i]["cdiscription"].ToString();
                    countrylist.Add(cou);

                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return countrylist;
            //return loadList();
            // return listdb();
        }



        [HttpGet("{cid}")]
        // [Route("getname")]
        public countryi getname(int cid)
        {
            try
            {
                if (cid <= 0)
                {
                    throw new Exception("please provide country id");
                }
            }
            catch (Exception e)
            {
                throw (e);
            }


            return countrydetails().Where(e => e.cid == cid).FirstOrDefault();
        }

        [HttpGet("checking_duplicatecountry")]
        public Boolean boole(string c_name, int c_id)
        {

            if (!string.IsNullOrEmpty(c_name))
            {

                bool t2 = returntype(c_name, c_id);
                if (t2)
                {
                    return true;
                }
            }
            return false;

        }
        [HttpPost("countryadd")]

        //[Route("countryadd")]
        public string countryadd(countryi Obj, int Optional)

        {
            try
            {
                if (string.IsNullOrEmpty(Obj.cname))
                {
                    throw new Exception("please provid the country name");
                }
                else if (!string.IsNullOrEmpty(Obj.cname))
                {
                    bool t2 = returntype(Obj.cname, (int)Obj.cid);
                    if (t2)
                    {
                        throw new Exception("entered name already exists");
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                        SqlCommand cmd = new SqlCommand("INSERT INTO countritable (cname,cdisplayname,cdiscription) values ('" + Obj.cname + "','" + Obj.cdisplayname + "','" + Obj.cdiscription + "')", con);
                        // using(SqlCommand cmd = new SqlCommand($"insert into employeetable values({Obj.employeeid},{Obj.employeename},{Obj.employeesalary)", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        //List<employee> emplist = new List<employee>();
                        //emplist.Add(Obj);
                    }
                }


            }
            catch (Exception e)
            {
                throw (e);
            }

            return "sucessfully added";

        }

        [HttpDelete("deletecountry")]
        //[Route("delete")]
        public string deletecountry(string cid)
        {
            try
            {
                if (string.IsNullOrEmpty(cid))
                {
                    throw new Exception("please select atleast one row");
                }
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand($"delete from countritable where cid in({cid})", con);
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

        [HttpPut("updatecountry")]
        // [Route("update")]
        public string updatecountry(countryi cou)

        {

            try
            {
                if (string.IsNullOrEmpty(cou.cname))
                {
                    throw new Exception("please provoid countryname");
                }
                else if (cou.cid <= 0)
                {
                    throw new Exception("please provoid valid country id");

                }
                else if (!string.IsNullOrEmpty(cou.cname))
                {


                    var query = @"UPDATE countritable SET cdisplayname = '" + cou.cdisplayname + @"',cdiscription = '" + cou.cdiscription + @"' ,cname = '" + cou.cname + @"'   where cid = '" + cou.cid + @"' ";

                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //DataTable dt = new DataTable();

                    // SqlDataAdapter da = new SqlDataAdapter(cmd);

                    // da.Fill(dt);


                }

            }
            catch (Exception e)
            {
                throw (e);
            }

            return "updated sucessfully";
        }
        [HttpGet("refcountry")]

        public Boolean refcountry(string c_id)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
            SqlCommand cmd = new SqlCommand($"select * from statetable where scountryid in({c_id})", con);

            con.Open();
            SqlDataReader read = cmd.ExecuteReader();
            if (read.HasRows)
            {
                return true;
            }

            return false;
        }

        private bool returntype(string c_name, int c_id)
        {
            var que = $"select cname,cid from countritable where cname= '{c_name}'";
            if (c_id > 0) que += $"and (cid <>{c_id}) ";
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("default"));

            SqlCommand cmdo = new SqlCommand(que, conn);

            conn.Open();
            SqlDataReader reader = cmdo.ExecuteReader();



            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    return true;
                }
            }


            reader.Close();
            conn.Close();
            return false;
        }
    }


}

