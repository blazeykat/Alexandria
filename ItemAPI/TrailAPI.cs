﻿using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Alexandria.ItemAPI
{
    static class TrailAPI
    {
        /// <summary>
        /// Adds a tiled trail to the Projectile
        /// </summary>
        /// <param name="timeTillAnimStart">How long after spawning until the trail will begin to play it's animation, if it has one.</param>
        public static void AddTrailToProjectile(this Projectile target, string spritePath, Vector2 colliderDimensions, Vector2 colliderOffsets, List<string> animPaths = null, int animFPS = -1, List<string> startAnimPaths = null, int startAnimFPS = -1, float timeTillAnimStart = -1, float cascadeTimer = -1, float softMaxLength = -1, bool destroyOnEmpty = false)
        {
            try
            {
                GameObject newTrailObject = new GameObject();
                FakePrefab.InstantiateAndFakeprefab(newTrailObject);
                newTrailObject.transform.parent = target.transform;

                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;

                int spriteID = SpriteBuilder.AddSpriteToCollection(spritePath, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = newTrailObject.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);

                tk2dSpriteAnimator animator = newTrailObject.GetOrAddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimation animation = newTrailObject.GetOrAddComponent<tk2dSpriteAnimation>();
                animation.clips = new tk2dSpriteAnimationClip[0];
                animator.Library = animation;

                TrailController trail = newTrailObject.AddComponent<TrailController>();

                //---------------- Sets up the animation for the main part of the trail
                if (animPaths != null)
                {
                    BeamAPI.SetupBeamPart(animation, animPaths, "trail_mid", animFPS, null, null, def.colliderVertices);
                    trail.animation = "trail_mid";
                    trail.usesAnimation = true;
                }
                else
                {
                    trail.usesAnimation = false;
                }

                if (startAnimPaths != null)
                {
                    BeamAPI.SetupBeamPart(animation, startAnimPaths, "trail_start", startAnimFPS, null, null, def.colliderVertices);
                    trail.startAnimation = "trail_start";
                    trail.usesStartAnimation = true;
                }
                else
                {
                    trail.usesStartAnimation = false;
                }

                //Trail Variables
                if (softMaxLength > 0) { trail.usesSoftMaxLength = true; trail.softMaxLength = softMaxLength; }
                if (cascadeTimer > 0) { trail.usesCascadeTimer = true; trail.cascadeTimer = cascadeTimer; }
                if (timeTillAnimStart > 0) { trail.usesGlobalTimer = true; trail.globalTimer = timeTillAnimStart; }
                trail.destroyOnEmpty = destroyOnEmpty;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }
        }
    }
}
