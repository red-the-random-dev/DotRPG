import ('DotRPG.Scripting', 'DotRPG.Objects')

event_counts = {default=1,treetalk=1,whiterectreset=1,treegone=1}

_manifest = {
    id="testDialog",
    useWith = "example/topdown",
}

punch_count = 0

function start()
    audio:BufferLocal("slide", "slide")
    audio:SetLooped("slide", true)
end

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if math.abs(obj:GetVelocityDerivative("whiterect")) > 32 then
            camera:Shake(10 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"), 50 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"))
            audio:PlayLocal("hit", math.min(0.8, 64 / obj:GetDistanceToPlayer("whiterect")), 1, obj:GetSoundPanning("whiterect"))
        end
        if obj:GetScalarVelocity("whiterect") > 128 then
            audio:SetParameters("slide", math.min(0.8, 64 / obj:GetDistanceToPlayer("whiterect")), obj:GetDopplerShift("whiterect"), obj:GetSoundPanning("whiterect"))
            audio:Play("slide")
        else
            audio:Stop("slide")
        end
        --error("Something is creating script errors")
    elseif objID == "treetalk" then
        timer:Enqueue(totalTime, 1000, "whiterectreset")
        audio:PlayLocal("hit")
        camera:Shake(10, 10)
        punch_count = punch_count + 1000
        if punch_count > 9000 then
            audio:PlayLocal("kaboom")
            obj:DisablePlayerControls()
            obj:SetAnimationSequence("tree", "explodes")
            timer:Enqueue(totalTime, 1000, "treegone")
        end
    elseif objID == "whiterectreset" then
        obj:SetObjectPosition("whiterect", 512, 270)
    elseif objID == "treegone" then
        obj:SetActive("tree", false)
        obj:EnablePlayerControls()
    end
    punch_count = punch_count - frameTime
    if punch_count < 0 then punch_count = 0 end
end