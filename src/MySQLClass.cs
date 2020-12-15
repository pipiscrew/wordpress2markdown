﻿using System;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DBManager.DBASES
{
    public class MySQLClass
    {

        private MySqlConnection objConn;
        private string m_ConnectionString;


        public MySQLClass(string ConnectionString, out MySqlException ExceptionObject)
        {
            try
            {
                //http://stackoverflow.com/questions/5754822/unable-to-convert-mysql-date-time-value-to-system-datetime
                m_ConnectionString = ConnectionString;
                objConn = new MySqlConnection(ConnectionString + ";CharSet=utf8;Allow Zero Datetime=True");
                objConn.Open();

                ExceptionObject = null;
            }
            catch (MySqlException ex)
            {
                objConn = null;
                ExceptionObject = ex;
            }
        }

        public bool IsConnected
        {
            get
            {
                //hotfix - https://stackoverflow.com/a/34357667/1320686
                if (objConn == null | !objConn.State.HasFlag(ConnectionState.Open)) //.State != ConnectionState.Open)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string ConnectionString
        {
            get { return m_ConnectionString; }
        }

        public MySqlDataAdapter GetAdapter(string sql)
        {
            return new MySqlDataAdapter(sql, objConn);
        }

        public MySqlCommand GetCommand(string Query)
        {
            return new MySqlCommand(Query, objConn);
        }

        public DataSet GetDATASET(string SQLSTR)
        {
            MySqlDataAdapter sqlAD = new MySqlDataAdapter();
            DataSet sqlSET = new DataSet();
            MySqlCommand sqlco = new MySqlCommand();

            try
            {
                sqlco.CommandText = SQLSTR;
                sqlco.Connection = objConn;

                sqlAD.SelectCommand = sqlco;
                sqlAD.Fill(sqlSET, "tabl");
                return sqlSET;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - GetDATASET");
                return null;
            }
            finally
            {
                sqlco.Dispose();
                sqlAD.Dispose();
                sqlSET.Dispose();
            }
        }


        public DataSet GetDATASET2(string SQLSTR, out string error)
        {
            MySqlDataAdapter sqlAD = new MySqlDataAdapter();
            DataSet sqlSET = new DataSet();
            MySqlCommand sqlco = new MySqlCommand();

            try
            {
                sqlco.CommandText = SQLSTR;
                sqlco.Connection = objConn;

                sqlAD.SelectCommand = sqlco;
                sqlAD.Fill(sqlSET, "tabl");
                error = "";
                return sqlSET;

            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
            finally
            {
                sqlco.Dispose();
                sqlAD.Dispose();
                sqlSET.Dispose();
            }
        }

        public MySqlDataReader GetDATAREADER(string SQLSTR)
        {
            MySqlDataReader sqlread = null;
            MySqlCommand sqlco = new MySqlCommand();
            try
            {
                sqlco.Connection = objConn;

                sqlco.CommandText = SQLSTR;

                sqlread = sqlco.ExecuteReader();
                return sqlread;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - GetDATAREADER");
                return null;
            }
            finally
            {
                sqlco.Dispose();
                //sqlread.Close()
            }
        }

        public DataTable GetDATATABLE(string SQLSTR)
        {
            MySqlDataAdapter sqlAD = new MySqlDataAdapter();
            DataTable sqlSET = new DataTable();
            MySqlCommand sqlco = new MySqlCommand();

            try
            {
                sqlco.CommandText = SQLSTR;
                sqlco.Connection = objConn;
                //sqlAD.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                sqlAD.SelectCommand = sqlco;
                sqlAD.Fill(sqlSET);

                return sqlSET;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - GetDATATABLE");
                return null;
            }
            finally
            {
                sqlco.Dispose();
                sqlAD.Dispose();
                sqlSET.Dispose();
            }
        }


        public DataTable GetDATATABLEex(string SQLSTR, out string Exception)
        {
            MySqlDataAdapter sqlAD = new MySqlDataAdapter();
            DataTable sqlSET = new DataTable();
            MySqlCommand sqlco = new MySqlCommand();

            Exception = null;

            try
            {
                sqlco.CommandText = SQLSTR;
                sqlco.Connection = objConn;

                sqlAD.SelectCommand = sqlco;
                sqlAD.Fill(sqlSET);

                return sqlSET;

            }
            catch (Exception ex)
            {
                Exception = ex.Message;
                return null;
            }
            finally
            {
                sqlco.Dispose();
                sqlAD.Dispose();
                sqlSET.Dispose();
            }
        }


        public MySqlConnection GetConnection()
        {
            return objConn;
        }


        public object ExecuteSQLScalar(string SQLSTR)
        {
            MySqlCommand sqlco = new MySqlCommand();
            try
            {
                sqlco.Connection = objConn;
                sqlco.CommandText = SQLSTR;
                return sqlco.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - ExecuteSQLScalar");
                return "";
            }
            finally
            {
                sqlco.Dispose();
            }
        }

        public int ExecuteNonQuery(string SQLSTR)
        {
            int functionReturnValue = 0;
            MySqlCommand sqlco = new MySqlCommand();
            try
            {
                sqlco.Connection = objConn;
                sqlco.CommandText = SQLSTR;
                return sqlco.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - ExecuteSQL");
                functionReturnValue = 0;
            }
            finally
            {
                sqlco.Dispose();
            }
            return functionReturnValue;
        }

        public int ExecuteNonQuery(string SQLSTR, out MySqlException ErrReport)
        {
            int functionReturnValue = 0;
            MySqlCommand sqlco = new MySqlCommand();
            ErrReport = null;

            try
            {

                sqlco.Connection = objConn;
                sqlco.CommandText = SQLSTR;

                return sqlco.ExecuteNonQuery();
            }
            catch (MySqlException sEX)
            {
                ErrReport = sEX;
                functionReturnValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MySQLClass - ExecuteSQL");
                functionReturnValue = 0;
            }
            finally
            {
                sqlco.Dispose();
            }
            return functionReturnValue;
        }

        public void Close()
        {
            if ((objConn != null))
            {
                objConn.Close();
                objConn.Dispose();
            }
        }


    }

}
