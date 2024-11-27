/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID AMBPIANO = 895127194U;
        static const AkUniqueID DESTROYEDGHOST = 3087263383U;
        static const AkUniqueID GHOSTCATCH = 387592991U;
        static const AkUniqueID GHOSTRELEASE = 1202154233U;
        static const AkUniqueID GHOSTSKILLCHECKFAIL = 3864087751U;
        static const AkUniqueID GHOSTSKILLCHECKPASS = 4233241136U;
        static const AkUniqueID PAINTCATCH = 2553231580U;
        static const AkUniqueID PAINTRELEASE = 3571119522U;
        static const AkUniqueID PIANOHIT = 469357355U;
        static const AkUniqueID START = 1281810935U;
        static const AkUniqueID START_LVL1 = 1123143573U;
        static const AkUniqueID STEP = 621108255U;
        static const AkUniqueID TRASHVACUUMED = 2644656531U;
        static const AkUniqueID VACSTART = 1719412607U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAMEMUSICCONTROL
        {
            static const AkUniqueID GROUP = 4231158115U;

            namespace STATE
            {
                static const AkUniqueID CATCHING = 1210588850U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID NORMAL = 1160234136U;
            } // namespace STATE
        } // namespace GAMEMUSICCONTROL

        namespace GAMESTATE
        {
            static const AkUniqueID GROUP = 4091656514U;

            namespace STATE
            {
                static const AkUniqueID CREDITS = 2201105581U;
                static const AkUniqueID MENU = 2607556080U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSE = 3092587493U;
                static const AkUniqueID PLAYING = 1852808225U;
            } // namespace STATE
        } // namespace GAMESTATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GAMEMUSIC
        {
            static const AkUniqueID GROUP = 1533192012U;

            namespace SWITCH
            {
                static const AkUniqueID GAMEMUSIC_01 = 1375774706U;
                static const AkUniqueID GAMEMUSIC_02 = 1375774705U;
                static const AkUniqueID GAMEMUSIC_03 = 1375774704U;
                static const AkUniqueID GAMEMUSIC_04 = 1375774711U;
                static const AkUniqueID GAMEMUSIC_05 = 1375774710U;
                static const AkUniqueID GAMEMUSIC_06 = 1375774709U;
            } // namespace SWITCH
        } // namespace GAMEMUSIC

        namespace ISWALKING
        {
            static const AkUniqueID GROUP = 3629409974U;

            namespace SWITCH
            {
                static const AkUniqueID NO = 1668749452U;
                static const AkUniqueID YES = 979470758U;
            } // namespace SWITCH
        } // namespace ISWALKING

        namespace STEPS
        {
            static const AkUniqueID GROUP = 1718617278U;

            namespace SWITCH
            {
                static const AkUniqueID CARPET = 2412606308U;
                static const AkUniqueID STOP = 788884573U;
                static const AkUniqueID TILE = 2637588553U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace SWITCH
        } // namespace STEPS

        namespace VACUUM
        {
            static const AkUniqueID GROUP = 3699816212U;

            namespace SWITCH
            {
                static const AkUniqueID HYDROWORKING = 4234065470U;
                static const AkUniqueID HYDROWORKINGONGOO = 2143811110U;
                static const AkUniqueID STOP = 788884573U;
                static const AkUniqueID VACWORKING = 2036889338U;
                static const AkUniqueID VACWORKINGONGHOST = 3149481222U;
            } // namespace SWITCH
        } // namespace VACUUM

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID GHOSTS = 1037543273U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
