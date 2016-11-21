module.exports = function (context, myTimer) {
    var timeStamp = new Date().toISOString();
    
    if(myTimer.isPastDue)
    {
        context.log('JavaScript is running late!');
    }
    context.log('JavaScript timer trigger function ran!', timeStamp);   
    var toBeQueued = new myQueueItem();
    toBeQueued.time=timeStamp;
    context.bindings.myQueueItem=toBeQueued;
    context.done();
};
function myQueueItem(){
    {
        return {
            msg:"messagem",
            time:"time"
        }
    }
}