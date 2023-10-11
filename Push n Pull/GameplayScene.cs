using Engine;
using Engine.DefaultComponents.Render.Primitive;
using Engine.External;
using Engine.SceneManager;
using Engine.TiledSharp;
using Engine.TiledSharp.Extension;
using Engine.Velcro;
using Genbox.VelcroPhysics.Dynamics;
using Raylib_cs;
using System.Numerics;

public class BoxesManager : SceneComponent
{
    public List<Box> boxes = new List<Box>();
    public List<BoxGoal> boxeGoals = new List<BoxGoal>();
}

public class GameplayScene : Scene
{
    public GameplayScene() 
        : base("gamplay", (int)((1280/720)*16 * mapScale * 20), (int)((1280 / 720) * 16*mapScale * 8), Color.BLACK, Color.BLACK)
    {}

    TmxMap tmxMap;
    TmxObjectGroup? entitiesGroup;
    static float mapScale = 1f;

    public override void OnLoad()
    {
        ContentManager.Load<rTexture>("character", @".asset/character.png");
        ContentManager.Load<rTexture>("box", @".asset/box.png");
        tmxMap = new TmxMap(@".asset/test.tmx");
    }
    public override void OnBegined()
    {
        Debugging.EnableDrawDebug = true;
        var foundEntitiesGroup = tmxMap.TryFindObjectGroup("entities",out entitiesGroup);

        var characterTexture = ContentManager.Get<rTexture>("character");
        var boxTexture = ContentManager.Get<rTexture>("box");

        ClearColor = tmxMap.BackgroundColor;

        CreateEntity("map")
            .Scale(mapScale)
            .AddComponent(new TileMap(tmxMap,"terrain"))
            .AddComponent(new TilemapBody("colliders",true))
            ;

        CreatePlayer(characterTexture);

        IEnumerable<TmxObject>? boxs = entitiesGroup?.Objects.Where(obj => obj.Type == "box");
        if (boxs != null) 
            foreach (TmxObject boxObj in boxs)
                CreateBox(boxTexture,boxObj);

        var box_goals = entitiesGroup.Objects.Where(obj => obj.Type == "box_goal");
        if (box_goals != null)
            foreach (TmxObject obj in box_goals)
                CreateBoxGoal(obj);

    }
    public override void OnUnload()
    {
        tmxMap.Unload();
    }

    void GetObjectData(TmxObject obj, ref Vector2 position, ref  Vector2 scale)
    {
        //scale
        scale = (obj.Width != 0 || obj.Height != 0) ?
            new Vector2((float)(obj.Width), (float)(obj.Height)) :
            scale;

        //position
        position = new Vector2((float)(obj.X), (float)(obj.Y));
        position += scale / 2f;
    }


    Entity CreatePlayer(rTexture texture)
    {
        var playerPosition = Vector2.Zero;
        var playerScale = new Vector2(20, 20);
        if (entitiesGroup != null)
        {
            GetObjectData(entitiesGroup.Objects["player"], ref playerPosition, ref playerScale);
            playerScale *= mapScale;
            playerPosition *= mapScale;
        }

        var gun = CreateEntity("gun_aim").Move(5,-5)
            
            ;

        CreateChildEntity(gun, "gun_visual")
            .AddComponent(new CircleRenderer(2, Color.WHITE))
            .AddComponent(new Trail())
            ;

        return CreateEntity("player")
            .SetProcessOrder(-1)
            ///* Tranform *///
            .MoveTo(playerPosition)
            .ScaleTo(16 / texture.Height)
            .Rotate(0, 0, 0)
            .Transform.AddChild(gun.Transform,false)
            .Entity
            ///* Physic *///
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Kinematic)
            .AddComponent(new VCollisionBox(playerScale))

            ///* Behavior *///
            .AddComponent(new KinematicController2D())
            .AddComponent(new Player())


             ///* Render *///
             .AddComponent(new SpriteRenderer(texture))
             .AddComponent(new SimpleLineRenderer())
             .Entity
            ;

        
    }

    Entity CreateBoxGoal(TmxObject objBoxGoal)
    {
        Vector2 boxPosition = Vector2.Zero;
        Vector2 boxScale = new Vector2(20, 20);

        if (entitiesGroup != null)
            GetObjectData(objBoxGoal, ref boxPosition, ref boxScale);

        var boxEn = CreateEntity(objBoxGoal.Name)
            .SetProcessOrder(-1)
            .MoveTo(boxPosition)
            .ScaleTo(boxScale.X, boxScale.Y, 1)

            ///* Physic *///
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Kinematic)
            .AddComponent(new VCollisionBox(boxScale))
                .SetSensor(true)

            ///* Behavior *///
            .AddComponent(new BoxGoal())

            ///* Render *///
             .AddComponent(new CircleRenderer(5,Color.GREEN))

            .Entity;
        return boxEn;
    }
    Entity CreateBox(rTexture texture,TmxObject objBox)
    {
        Vector2 boxPosition = Vector2.Zero;
        Vector2 boxScale = new Vector2(20, 20);
        if (entitiesGroup != null)
            GetObjectData(objBox, ref boxPosition, ref boxScale);


        var boxEn = CreateEntity(objBox.Name)
            .SetProcessOrder(-1)
            ///* Tranform *///
            .MoveTo(boxPosition)
            .ScaleTo(16 / texture.Height)
            .Rotate(0, 0, 0)

            ///* Physic *///
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Kinematic)
            .AddComponent(new VCollisionBox(boxScale))

            ///* Behavior *///
            .AddComponent(new KinematicController2D())
            .AddComponent(new Box())

             ///* Render *///
             .AddComponent(new SpriteRenderer(texture))
             .Entity
            ;

        return boxEn;
    }
}

