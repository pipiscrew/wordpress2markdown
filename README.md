# wordpress2markdown

A small tool, to convert wordpress database to markdown, primarily to be used with Jekyll on Github.

For the best results download the database locally.

lines need to be adjusted for table names : 
55/93/94/95/100

--

## Github workflow reference :

-Dont forget YAML is whitespace sensitive always validate with https://yamlvalidator.com/

-Adjust the timeout by providing 'timeout-minutes'
```javascript
jobs:
  continuous-delivery:

    runs-on: ubuntu-latest
    timeout-minutes: 1200
```
	
## Jekyll reference :

jekyll serve - build & start local server

jekyll clean - clean any garbage



# This project uses the following 3rd-party dependencies :<br>
-[Html2Markdown.dll](https://github.com/baynezy/Html2Markdown)<br>
-[HtmlAgilityPack.dll](https://www.nuget.org/packages/HtmlAgilityPack/1.5.0)<br>
-[MySql.Data.dll](https://dev.mysql.com)<br>
<br><br><br>
## This project is no longer maintained
Copyright (c) 2020 [PipisCrew](http://pipiscrew.com)

Licensed under the [MIT license](http://www.opensource.org/licenses/mit-license.php).