event_counts = {default=1,treetalk=1,whiterectreset=1,treegone=1}

_manifest = {
    id="testDialog",
    useWith = "example/topdown",
}

punch_count = 0

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        punch_count = punch_count - frameTime
        if punch_count < 0 then punch_count = 0 end
    end
    if objID == "treetalk" then
        timer:Enqueue(totalTime, 1000, "kickablereset")
        audio:PlayLocal("hit")
        camera:Shake(10, 10)
        punch_count = punch_count + 1000
        if punch_count > 9000 then
            audio:PlayLocal("kaboom")
            obj:DisablePlayerControls()
            obj:SetAnimationSequence("tree", "explodes")
            timer:Enqueue(totalTime, 1000, "treegone")
        end
    end
    if objID == "treegone" then
        obj:SetActive("tree", false)
        obj:EnablePlayerControls()
    end
end