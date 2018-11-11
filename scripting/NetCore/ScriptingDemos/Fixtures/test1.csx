using System;

class Message
{
    public string Text { get; set; }
}

var message = new Message
{
    Text = "hello world"
};

Console.WriteLine(message.Text);