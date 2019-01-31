const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp(functions.config().firebase);
const db = admin.database();

exports.createRoom = functions.https.onRequest((req, res) =>
{
    var room = req.body.room;

    db.ref("/Rooms/" + room).set({gsr:0, hr:0}).then((snapshot) =>
    {
        res.send("OK");
        return;
    }).catch(error => {res.status(500).send("ERR")});
});

exports.updateValues = functions.https.onRequest((req, res) =>
{
    const room = req.body.room;
    const hr = Number(req.body.hr);
    const gsr = Number(req.body.gsr);

    db.ref("/Rooms").child(room).once("value").then((snapshot) =>
    {
        if(snapshot.exists())
        {
            return db.ref("/Rooms/" + room).update({'hr': hr, 'gsr': gsr});
        }
        throw new Error("Error");
    }).then((snapshot) =>
    {
        if(snapshot !== null)
        {
            res.send("OK");
            return;
        }
        else throw new Error("Error");
    }).catch(error => {res.status(500).send("ERR")});

});

exports.addQuestion = functions.https.onRequest((req, res) =>
{
    const room = req.body.room;
    const question = req.body.question;
    console.log(room + " " + question);

    db.ref("/Rooms").child(room).once("value").then((snapshot) =>
    {
        console.log(snapshot.val().hr);
        if(snapshot.exists())
        {
            return db.ref("/Question/" + room).push().set(question);
        }
        throw new Error("Error");

    }).then((snapshot) =>
    {
        if(snapshot !== null)
        {
            res.send("OK");
            return;
        }
        else throw new Error("Error");

    }).catch(error => {res.status(500).send("ERR")});
});

exports.removeRoom = functions.https.onRequest((req, res) =>
{
    const room = req.body.room;

    db.ref("/Rooms").child(room).remove();
    db.ref("/Questions").child(room).remove();

    res.status(200).send();
})
