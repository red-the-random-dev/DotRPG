import ('DotRPG.Scripting', 'DotRPG.Objects')

event_counts = {default=1,treetalk=7}

_manifest = {
    id="testDialog",
    useWith = "example/topdown",
}

shake_count = 0

function start()
    audio:BufferLocal("slide", "slide")
    audio:SetLooped("slide", true)
end

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if obj:GetVelocityDerivative("whiterect") < -32 then
            camera:Shake(10 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"), 50 * math.abs(obj:GetVelocityDerivative("whiterect")) / obj:GetDistanceToPlayer("whiterect"))
            audio:PlayLocal("hit", math.min(0.8, 64 / obj:GetDistanceToPlayer("whiterect")), 1, obj:GetSoundPanning("whiterect"))
        end
        if obj:GetScalarVelocity("whiterect") > 128 then
            audio:SetParameters("slide", math.min(0.8, 64 / obj:GetDistanceToPlayer("whiterect")), 1, obj:GetSoundPanning("whiterect"))
            audio:Play("slide")
        else
            audio:Stop("slide")
        end
        --error("Something is creating script errors")
    end
end