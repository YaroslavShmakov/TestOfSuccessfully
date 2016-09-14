var res = [];
var text = [];
var ansTest = [];
var first = 0;
var id;
var ready = 0;
var i = 0;

	function Subscribe()
    {
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/Subscribe/",
            contentType: "application/json; charset=utf-8",
            success: function (arg) {
				window.id = arg;
            }
        })
    }

    function Unsubscribe(id)
    {
        var arg = "{'id':" + id + "}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/Unsubscribe/",
            contentType: "application/json; charset=utf-8",
            data: arg,
            success: function (arg) {
                $("#result").text(arg.data);
            }
        })
    }
    
    function Unsubscr()
    {
        Unsubscribe(id);
        setDefault();
    }
	
    function setDefault()
    {
        clearMas(res);
        clearMas(text);
        clearMas(ansTest);
        first = 0;
        ready = 0;
        i = 0;
    }


    function CreateTests(id, result)
    {
        var arg = "{'id':" + id + ", 'result':[" + result + "]}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/CreateTests/",
            contentType: "application/json; charset=utf-8",
            data: arg
        })
    }
	
    function TestsReady(id)
    {
        var arg = "{'id':" + id + "}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/TestsReady/",
            contentType: "application/json; charset=utf-8",
            data: arg,
            success: function (arg) {
                window.ready = arg;
            }
        })
    }
	
	function getReadyTest()
	{
		var timerId = setInterval(function() {
			if(ready)
			{
				clearInterval(timerId);
				clearInterval(timer);
				GetQuestions(id);
				ready = 0;
			}
			else
				TestsReady(id);
		}, 300);

		var timer = setTimeout(function() {
		clearInterval(timerId);
		alert("Сервер перегружен. Попробуйте пройти тест ещё раз через пару минут.");
		}, 15000);
	}

    function GetQuestions(id)
    {
        var arg = "{'id':" + id + "}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/GetQuestions/",
            contentType: "application/json; charset=utf-8",
            data: arg,
            success: function (arg) {
				window.text = arg;
            }
        })
    }

    function GetGradeLucky(id, result)
    {
        var arg = "{'id':" + id + ", 'result':[" + result + "]}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/GetGradeLucky/",
            contentType: "application/json; charset=utf-8",
            data: arg
        })
    }

    function ResultsReady(id) {
        var arg = "{'id':" + id + "}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/ResultsReady/",
            contentType: "application/json; charset=utf-8",
            data: arg,
            success: function (arg) {
                window.ready = arg;
            }
        })
    }

    function GetFinishResult(id)
    {
        var arg = "{'id':" + id + "}";
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "http://localhost:8002/api/RESTReciver/GetFinishResult/",
            contentType: "application/json; charset=utf-8",
            data: arg,
            success: function (arg) {
               window.res = arg;
            }
        })
    }

function toggle(el)
{
 	el.style.display = (el.style.display == 'none') ? 'block' : 'none'
}

function color_click(color_code)
{
	res.push(color_code);
	
	if (res.length == 8)
	{
		if(first)
		{
			CreateTests(id, res);
			toggle(instrTest);
			toggle(test_lusher);
			getReadyTest(id);
		}
		else
		{
			++first;
			clearMas(res);
			toggle(test_lusher);
			toggle(instructions);
		}
	}
}

function showLusherTest()
{
    toggle(test_lusher);
    toggle(black_color);
    toggle(blue_color);
    toggle(fiol_color);
    toggle(kor_color);
    toggle(ser_color);
    toggle(green_color);
    toggle(rad_color);
    toggle(yellow_color);
}

function clearMas(mas)
{
	while(mas.length)
	{
		mas.pop();
	}
}

function getReadyLucky()
{
	var timerId2 = setInterval(function() {
		if(ready)
		{
			clearInterval(timerId2);
			clearInterval(timer2);
			GetFinishResult(id);
			toggle(show);
		}
		else
			ResultsReady(id);
	}, 300);

	var timer2 = setTimeout(function() {
	clearInterval(timerId);
	alert("Сервер перегружен. Попробуйте пройти тест ещё раз через пару минут.");
	}, 15000);
}

function nextQuestion()
{
	if (i < text.length)
	{
		document.getElementById("question").innerText = text[i];
		++i;
	}	
	else
	{
		GetGradeLucky(id,ansTest);
		toggle(charactTest);
		getReadyLucky();
        i = 0;
	}
}

function Show()
{
	document.getElementById("show").innerHTML = res;
}

function Yes()
{
	ansTest.push(1);
	nextQuestion();
}

function No()
{
	ansTest.push(0);
	nextQuestion();
}