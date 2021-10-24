event_counts = {default=1, kickablereset=1}

_manifest = {
    id="kickable",
    useWith = "example/topdown",
}

og_pos_x = 0
og_pos_y = 0

function start()
    audio:BufferLocal("slide", "slide__"..this)
    audio:SetLooped("slide__"..this, true)
    og_pos_x = obj:Pos_X(this)
    og_pos_y = obj:Pos_Y(this)
    event_counts["kick__"..this] = 1
end

function update(objID, eventID, frameTime, totalTime)
    if objID == "default" then
        if obj:GetScalarVelocity(this) > 128 then
            audio:SetParameters("slide__"..this, math.min(0.8, 64 / obj:GetDistanceToPlayer(this)), obj:GetDopplerShift(this), obj:GetSoundPanning(this))
            audio:Play("slide__"..this)
        else
            audio:Stop("slide__"..this)
        end
        if math.abs(obj:GetVelocityDerivative(this)) > 32 then
            camera:Shake(10 * math.abs(obj:GetVelocityDerivative(this)) / obj:GetDistanceToPlayer(this), 50 * math.abs(obj:GetVelocityDerivative(this)) / obj:GetDistanceToPlayer(this))
            audio:PlayLocal("hit", math.min(0.8, 64 / obj:GetDistanceToPlayer(this)), 1, obj:GetSoundPanning(this))
        end
    end
    if objID == "kickablereset" then
        obj:SetObjectPosition(this, og_pos_x, og_pos_y)
    end
    if objID == "kick__"..this then
        obj:ApplyForce(this, 256*15, obj:GetAzimuthFromPlayer(this))
    end
end