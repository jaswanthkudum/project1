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
    public class stateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public stateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("statedetails")]
        //[Route("countrydetails")]


        public List<statei> statedetails()
        {

            List<statei> statelist = new List<statei>();
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand("SELECT countritable.cname, statetable.scountryid, statetable.sdiscription, statetable.sdisplayname, statetable.sname, statetable.stid FROM statetable Inner JOIN countritable ON statetable.scountryid = countritable.cid", con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    statei cou = new statei();
                    cou.stid = Int32.Parse(dt.Rows[i]["stid"].ToString());

                    cou.sname = dt.Rows[i]["sname"].ToString();
                    cou.cname = dt.Rows[i]["cname"].ToString();
                    cou.sdisplayname = dt.Rows[i]["sdisplayname"].ToString();
                    cou.sdiscription = dt.Rows[i]["sdiscription"].ToString();
                    cou.scountryid = Int32.Parse(dt.Rows[i]["scountryid"].ToString());
                    statelist.Add(cou);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return statelist;
            //return loadList();
            // return listdb();


        }


        [HttpGet("{stid}")]
        // [Route("getname")]

        public statei getname(int stid)
        {
            try
            {
                if (stid <= 0)
                {
                    throw new Exception("please provide state id");
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return statedetails().Where(e => e.stid == stid).FirstOrDefault();
        }


        [HttpPost("stateadd")]

        //[Route("countryadd")]
        public string stateadd(statei obj)

        {
            try
            {

                if (string.IsNullOrEmpty(obj.sname))
                {
                }
                //else if (obj.scountryid <= 0)
                //{
                //    throw new Exception("please provide valid country id");
                //}
                else if (!string.IsNullOrEmpty(obj.sname))
                {
                    bool t2 = returntype(obj.sname, obj.stid, obj.scountryid);
                    if (t2)
                    {
                        throw new Exception("entered value already exists");
                    }
                    else if (obj.scountryid <= 0)
                    {
                        throw new Exception("please provide valid country id");
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                        SqlCommand cmd = new SqlCommand("INSERT INTO statetable (sname,sdisplayname,sdiscription,scountryid) VALUES ('" + obj.sname + "','" + obj.sdisplayname + "','" + obj.sdiscription + "','" + obj.scountryid + "')", con);
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
        [HttpDelete("deletestate")]
        //[Route("delete")]
        public string deletestate(string stid)
        {
            try
            {
                if (string.IsNullOrEmpty(stid))
                {
                    throw new Exception("please select atleast one row");
                }

                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
                SqlCommand cmd = new SqlCommand($"delete from statetable where stid in({stid})", con);
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

        [HttpPut("updatestate")]
        // [Route("update")]
        public string updatestate(statei co)

        {
            try
            {
                if (string.IsNullOrEmpty(co.sname))
                {
                    throw new Exception("please provide the state name");
                }
                else if (co.scountryid <= 0)
                {
                    throw new Exception("please provide valid country id");
                }
                else if (co.stid <= 0)
                {
                    throw new Exception("please provide valid state  id");
                }
                else if (!string.IsNullOrEmpty(co.sname))
                {


                    var query = @"UPDATE statetable SET sname='" + co.sname + @"',sdisplayname='" + co.sdisplayname + @"',scountryid='" + co.scountryid + @"',sdiscription='" + co.sdiscription + @"'   where stid = '" + co.stid + @"' ";
                    SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //DataTable dt = new DataTable();

                    //SqlDataAdapter da = new SqlDataAdapter(cmd);

                    //da.Fill(dt);

                }

            }
            catch (Exception e)
            {
                throw (e);
            }


            return "updated sucessfully";
        }
        [HttpGet("checking_duplicatestate")]
        public Boolean boole(string s_name, int s_countryid, int st_id)
        {

            if (!string.IsNullOrEmpty(s_name))
            {

                bool t2 = returntype(s_name, s_countryid, st_id);
                if (t2)
                {
                    return true;
                }
            }
            return false;

        }
        [HttpGet("refstate")]

        public Boolean refstate(string st_id)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("default"));
            SqlCommand cmd = new SqlCommand($"select * from citytable where cistateid in({st_id})", con);

            con.Open();
            SqlDataReader read = cmd.ExecuteReader();
            if (read.HasRows)
            {
                return true;
            }

            return false;
        }

        private bool returntype(string s_name, int s_countryid, int st_id)
        {
            var que = $"select sname,stid,scountryid from statetable where sname= '{s_name}'and scountryid={s_countryid} ";
            //if (st_id != 0 ||s_countryid != 0)
            //{
            //    que += $"and((stid<>{st_id} or stid={st_id}) or (scountryid<>{s_countryid} or scountryid={s_countryid})) ";
            //}
            if (st_id > 0)
            {
                que += $"and (stid <>{st_id}) ";
            }

            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("default"));

            SqlCommand cmdo = new SqlCommand(que, conn);

            conn.Open();
            SqlDataReader reader = cmdo.ExecuteReader();


            while (reader.Read())
            {
                //if ((int)reader["stid"] == st_id && (string)reader["sname"] == s_name && (int)reader["scountryid"] == s_countryid)
                //{
                //    return false;

                //}
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
