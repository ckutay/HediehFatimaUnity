/*
The crGameKeys class was created with one basic idea in mind:
Anything you could ever do in a game boils down to one of two choices:Yes or No

Do you have any money? Have you receive the third mission yet? Did you kill the bear?
Did you complete the previous task? Are you now allowed to pass the great gate?
Etc etc etc.

With this in mind I created the crGameKeys class to keep track of a game's progress.
If you achieve something, key it. Wether it is good or bad, key it and you will always
know where you are and what you have done. Picked up 1000 gold? Simply create a 'gold'
key and give it a value of 1000. Lost 1000 gold? Simply remove 1000 from your available
'gold' keys. If you don't have any keys of a particular type, remove the key altogether.

To use this system to keep track of all your progress, simply create a crGameKeys variable
inside any of your scripts and use it with the following functions... 

FUNCTIONS
========
add(name: String, value: int)
subtract(name: String, value: int) : int
doesHave(name: String, amt: int) : boolean
doesNotHave(name: String, amt: int) : boolean

EXAMPLE USAGE:
==============
var PartyInventory	: crGameKeys;
var GeneralKeys		: crGameKeys;

PartyInventory = new crGameKeys();
PartyInventory.add("Fire Spell", 10);
PartyInventory.add("Phoenix Down", 1);

if (PartyInventory.doesHave("Fire Spell",10))
	PartyInventory.subtract("Fire Spell", 5);
	
GeneralKeys = new crGameKeys();
if (GeneralKeys.doesNotHave("DidTutorial", 1)
{
	DoTutorial();
	GeneralKeys.add("DidTutorial", 1);
}

SUMMARY
=======
The crGameKeys class automatically handles the creation and deletion of keys so all you
have to do is add or subtract them. When you want to see if you have any, just use the two
functions mentioned above to test if you have the required amount.
There is nothing more to do on your part...

doesHave(name: String, amt: int)
Returns true if the number of the keys held are equal to or greather than amt.
Returns false if the key is not found.

doesNotHave(name: String, amt: int)
Returns true if the number of keys held are less than amt.
Returns true of the key is not found.

*/

class crKeyVal extends Object {

private var name	: String;
private var value	: int;

function crKeyVal()
{
		name	= "empty";
		value	= -1;
}

function crKeyVal(nm: String, val: int)
{
    name = nm;
    value = val;
}

function getName() : String
{
	return name;
}

function getValue() : int
{
	return value;
}

function setValue(val: int)
{
	value = val;
}

function setName(nm : String)
{
	name = nm;
}

function add(amount: int)
{
    value += amount;
}

function subtract(amount: int)
{
    value -= amount;
}

}

class crGameKeys
{
var allKeys 					: Array;
var atIndex 					: int;
var lastSearchSuccess		 	: boolean;
var currentKey 					: crKeyVal;
var caseSensitive 				: boolean;

function crGameKeys()
{
	allKeys = new Array();
	atIndex = -1;
	lastSearchSuccess = false;
	caseSensitive = false;
}

function setSearchResults(result, key)
{
	lastSearchSuccess = result;
    atIndex = key;
    
    if (key == -1)
    currentKey = null; else
    currentKey = allKeys[key];
}

function find(name: String) : int
{
	var abc : String;
	var tmp : crKeyVal;

	if (!caseSensitive)
		name = name.ToLower();
		
	if (allKeys.length > 0)
	{
      for (var x=0; x < keyCount(); x++)
      {
      	tmp = allKeys[x];
      	if (!caseSensitive)
      		 abc = tmp.getName().ToLower();
      	else abc = tmp.getName();
      	
        if (abc == name)
        {
        	setSearchResults(true, x);
            return atIndex;
        }
      }

       setSearchResults(false, keyCount() - 1);
	} else
	{
       setSearchResults(false, -1);
	}

    return atIndex;
}

function findByIndex(id: int) : int
{
	if (keyCount() == 0)
	{
		setSearchResults(false, -1);
	} else
	{
		if (id >= keyCount() ) id = 0;
		setSearchResults(true, id);
	}
	
	return atIndex;
}

function keyFound()
{
	return lastSearchSuccess;
}

function firstKey()
{
	if (keyCount() > 0)
		 setSearchResults(true, 0);
	else setSearchResults(false, -1);
	
	return currentKey;
}

function lastKey()
{
	if (keyCount() > 0)
		 setSearchResults(true, keyCount() -1);
	else setSearchResults(false, -1);
	
	return currentKey;
}

function add(name: String, value: int)
{
    find(name);
    
    if ( keyFound() )
    {
        currentKey.add(value);
    } else
    {
    var newKey : crKeyVal = new crKeyVal(name, value);
        allKeys.push(newKey);
        currentKey = lastKey();
        atIndex = keyCount() - 1;
    }
}

function subtract(name: String, value: int) : int
{
	if ( find(name) > -1)
	{
		if (currentKey.getValue() > value)
		{
			currentKey.subtract(value);
			return currentKey.getValue();
		} else
		{
			removeCurrentKey();
			return -1;
		}
	}
}

function removeCurrentKey()
{
	allKeys.RemoveAt(atIndex);
	
    if (atIndex >= allKeys.length)
    	atIndex = allKeys.length - 1;
    	
    if (allKeys.length == 0)
    {
    	currentKey = null;
    	atIndex = -1;
    } else
    {
    	currentKey = allKeys[atIndex];
    }
}

function Clear()
{
	allKeys.Clear();	
	currentKey = null;
	atIndex = -1;
}

function remove(name: String)
{    
    if (find(name) > -1)
    {
            removeCurrentKey();
    }
}

function keyCount() : int
{
	return allKeys.length;
}


function doesHave(name: String, amt: int) : boolean
{
	find(name);
	if ( !keyFound() ) return false;
	
	if (currentKey.getValue() >= amt)
		 return true;
	else return false;
}

function doesNotHave(name: String, amt: int) : boolean
{
	find(name);
	if ( !keyFound() ) return true;
	
	if (currentKey.getValue() < amt)
		 return true;
	else return false;
}

}