//using system.collections.generic;
//using tmpro; // textmeshpro ui 사용
//using unityengine;
//using system.io;

//public class armyeditormanager : monobehaviour
//{
//    [header("game data")]
//    // [1] 편집 가능한 모든 유닛의 원본(so) 리스트
//    public list<unitso> allavailableunits;

//    [header("point system")]
//    public int maxarmypoints = 2000; // 부대 최대 포인트

//    [header("ui references")]
//    public transform availableunitslistparent; // 사용 가능한 유닛 목록이 표시될 ui 부모
//    public transform currentarmylistparent;   // 현재 부대에 포함된 유닛 목록이 표시될 ui 부모
//    public textmeshprougui pointstext;         // "현재 포인트: 1500 / 2000"

//    // (ui 버튼 프리팹들)
//    // public gameobject unitselectbuttonprefab;
//    // public gameobject troopentryprefab;

//    // [2] 현재 편집 중인 부대 정보 (메모리)
//    private playerarmy currentarmy;

//    void start()
//    {
//        currentarmy = gamedataholder.armytoedit;
//        if (currentarmy == null ) currentarmy = new playerarmy();

//        // (여기서 allavailableunits 리스트를 기반으로 ui 버튼들 생성)

//        updateui();
//    }

//    // [4] 유닛을 부대에 추가 (ui 버튼에서 호출)
//    public void addunit(unitso unittoadd)
//    {
//        int newtotalpoints = currentarmy.totalcosts + unittoadd.pointcost;

//        // 포인트 제한 체크
//        if (newtotalpoints > maxarmypoints)
//        {
//            debug.logwarning("포인트 초과! 유닛을 추가할 수 없습니다.");
//            return;
//        }

//        // 데이터 업데이트
//        currentarmy.totalcosts = newtotalpoints;
//        currentarmy.units.add(new armyunitentry(unittoadd.unitid));

//        // ui 갱신
//        updateui();
//    }

//    // [5] 부대에서 유닛 제거 (ui 버튼에서 호출)
//    public void removeunit(armyunitentry unittoremove)
//    {
//        // (unittoremove.unitid를 사용해 unitdefinitionso를 찾아서 pointcost를 가져와야 함)
//        // unitdefinitionso unitso = findunitso_byid(unittoremove.unitid);
//        // currentarmy.totalpointsused -= unitso.pointcost;
//        // currentarmy.units.remove(unittoremove);

//        updateui();
//    }

//    // [6] ui 갱신 (유닛 목록, 포인트 텍스트 등)
//    private void updateui()
//    {
//        // (currentarmylistparent의 자식 ui들을 갱신)

//        pointstext.text = $"현재 포인트: {currentarmy.totalcosts} / {maxarmypoints}";
//    }

//    // [7] 저장 버튼 (ui 버튼에서 호출)
//    public void onsavebuttonclicked()
//    {
//        saveloadservice.savearmy(currentarmy);
//    }
//}