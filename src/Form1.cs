using DBManager.DBASES;
using Html2Markdown;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//dependencies
//https://github.com/baynezy/Html2Markdown
//https://www.nuget.org/packages/HtmlAgilityPack/1.5.0

/*
 * query by category per count
select post_title, w3.name, post_modified, CAST(pm.meta_value AS int) as post_count from w_posts  p
left join w_term_relationships w on w.object_id = p.id
left join w_term_taxonomy w2 on w2.term_taxonomy_id = w.term_taxonomy_id	
left join w_terms w3 on w3.term_id = w2.term_id
left join w_postmeta pm on pm.post_id = p.id and meta_key='_post_views'
where post_type = 'post' and post_status = 'publish' and w3.name='news'
order by post_count desc, post_modified desc
 */
namespace wordpress2markdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Text = Application.ProductName + " v" + Application.ProductVersion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlException err = null;
            General.db = new MySQLClass("Data Source=" + textBox1.Text.Trim() +
                    ";Port=" + textBox1b.Text.Trim() +
                    ";Initial Catalog=" + textBox4.Text.Trim() +
                    ";User ID=" + textBox2.Text.Trim() +
                     ";Password=" + textBox3.Text.Trim(), out err);

            if (err != null)
            {
                General.db = null;
                General.Mes(err.Message, MessageBoxIcon.Error);
            }
            else
            {
                string recordStatusSQL = @"select post_type, count(*) as records from w_posts
                                                where post_status = 'publish' and post_modified between '{0}' and '{1}'
                                                group by post_type
                                                order by records desc";

                DataTable dt = General.db.GetDATATABLE(string.Format(recordStatusSQL, dtp1.Value.ToString("yyyy-MM-dd"), dtp2.Value.ToString("yyyy-MM-dd")));

                dt.Columns.Add("Export", typeof(Boolean)).SetOrdinal(0);
                dt.Columns[1].ReadOnly = true;
                dt.Columns[2].ReadOnly = true;
                dg.DataSource = dt;
                dg.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                groupBox1.Enabled = false;
                btnCancel.Visible = groupBox2.Enabled = !groupBox1.Enabled;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var g = dg.Rows.Cast<DataGridViewRow>()
                                                .Where(r => r.Cells[0].Value.ToBool()).Select(x => x.Cells[1].Value).ToList();

            if (g.Count == 0)
            {
                General.Mes("Please select a post_type by the grid", MessageBoxIcon.Exclamation);
                return;
            }

            string path = textBox5.Text.Trim();
            string author = textBox6.Text.Trim();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string wherePostType = "('" + string.Join("','", g) + "')";

            string articleSQL = @"select DATE_FORMAT(post_modified, '%Y-%m-%d') as dateMod, post_title, post_content, (
	                                    select GROUP_CONCAT(w3.name) from w_term_relationships w
	                                    left join w_term_taxonomy w2 on w2.term_taxonomy_id = w.term_taxonomy_id	
	                                    left join w_terms w3 on w3.term_id = w2.term_id
	                                    where w.object_id = p.id
                                    ) as categories
                                    , CONCAT(guid, ' ', post_name) as ref

                                    from w_posts  p
                                    where post_type in {0} and post_status = 'publish' and post_modified between '{1}' and '{2}' order by id desc";

            DataTable dt = General.db.GetDATATABLE(string.Format(articleSQL, wherePostType, dtp1.Value.ToString("yyyy-MM-dd"), dtp2.Value.ToString("yyyy-MM-dd")));

            //markdown  
            Converter converter = new Converter();
            string articleTemplate = @"---
title: {0}
author: {1}
date: {2}
categories: [{3}]
toc: true
---

";
            int succeed = 0;
            int emptyTitles = 0;
            List<string> errors = new List<string>();
            int titleMaxLength = int.Parse(txtMaxLength.Value.ToString());

            foreach (DataRow r in dt.Rows)
            {
                string dateMod = r.Field<string>("dateMod");
                string title = r.Field<string>("post_title").Trim();
                string html = r.Field<string>("post_content");
                string categories = r.Field<string>("categories");
                string reference = r.Field<string>("ref");

                if (string.IsNullOrEmpty(title))
                {
                    errors.Add("*null title* " + reference);
                    continue;
                }

                string content = WebUtility.HtmlDecode(html).Trim(); //convert content to real HTML

                try
                {
                    //convert HTML to Markdown                    
                    content = converter.Convert(content);
                }
                catch
                {
                    errors.Add("*markdown conversion failed* " + reference);
                    continue;
                }

                //replace 'SyntaxHighlighter Evolved' syntax with rouge
                content = Regex.Replace(content, @"(\[(.*)highlight(.*)\])|(\[php\])|(\[csharp\])|(\[sql\])|(\[js\])|(\[vb\])|(\[ps\])|(\[java\])|(\[html\])|(\[xml\])|(\[bash\])|(\[css\])", "```js");
                content = Regex.Replace(content, @"(\[/php\])|(\[/csharp\])|(\[/sql\])|(\[/js\])|(\[/vb\])|(\[/ps\])|(\[/java\])|(\[/html\])|(\[/xml\])|(\[/bash\])|(\[/css\])", "```");

                //validate TitleToken starts with letter
                string validTokenTitle = title;
                if (!char.IsLetter(validTokenTitle[0]))
                    validTokenTitle = "o" + validTokenTitle;

                validTokenTitle = validTokenTitle.Replace(":", "-").Replace("\"", "-").Replace("'", "-");

                content = string.Format(articleTemplate, validTokenTitle, author, dateMod, categories) + content + "\r\n\r\norigin - " + reference;

                //validate unwanted content that breaks YAML
                if (content.Contains("<%="))
                {
                    errors.Add("*invalid article content <%=* " + reference);
                    continue;
                }

                //prepare export filename, when is empty generate short guid
                title = title.Replace(" ", "-").MakeFilenameOnlyAlphaNumeric().Replace("--", "-").ShorterExact(titleMaxLength).ToLower().Greek2Greeklish();

                if (string.Empty.Equals(title))
                {
                    emptyTitles++;
                    title = "a" + Guid.NewGuid().ToString("n").Substring(0, 8);
                }

                string filename = string.Format("{0}-{1}", dateMod, title);
                string location = string.Format("{0}\\{1}.md", path, filename);
                File.WriteAllText(location, content, new UTF8Encoding(false));

                succeed++;
            }

            if (errors.Count > 0)
                File.WriteAllText(path + "\\!errors.txt", string.Join("\r\n", errors), new UTF8Encoding(false));


            General.Mes("Succeed : " + succeed + "\r\nEmpty titles fixed : " + emptyTitles + "\r\nErrors : " + errors.Count);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            btnCancel.Visible = groupBox2.Enabled = !groupBox1.Enabled;
            dg.DataSource = null;
        }


    }
}
