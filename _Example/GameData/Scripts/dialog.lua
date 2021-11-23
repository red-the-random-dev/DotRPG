event_counts = {default=1,banana_start=1,banana_talk=11}

_manifest = {
    id="testDialog",
    useWith = "example/topdown",
}
active = false

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if active == false then
            timer:Enqueue(totalTime, 1000, "banana_start")
            active = true
        end
    end
    if objID == "banana_start" then
        dialogue:Insert("banana_talk")
        dialogue:SetNormalSpeed(8)
        dialogue:SetAltSpeed(16)
        dialogue:SetFlags(true, false, false)
    end
    if objID == "banana_talk" then
        if eventID == 0 then
            dialogue:Show("bananas\nrotat e", false, true)
        elseif eventID == 1 then
            dialogue:Show("banan a", false, true)
        elseif eventID == 2 then
            dialogue:Show("rotato faster", false, true)
        elseif eventID == 3 then
            dialogue:Show("banana", false, true)
        elseif eventID == 4 then
            dialogue:Show("go", false, true)
        elseif eventID == 5 then
            dialogue:Show("g     O", false, true)
        elseif eventID == 6 then
            dialogue:Show("can you fEEl it ", false, true)
        elseif eventID == 7 then
            dialogue:Show("banan an\nba", false, true)
        elseif eventID == 8 then
            dialogue:Show("ugh", true, true)
        elseif eventID == 9 then
            dialogue:Show("yES FEEL THE SPED", false, true)
        elseif eventID == 10 then
            dialogue:Show("WE HAVE REAHCED MXAIMUN VELOCIPY", false, false)
        end
    end
end